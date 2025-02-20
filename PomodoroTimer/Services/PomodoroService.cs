using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using PomodoroTimer.Models;
using System.Windows.Forms;  // 添加这个引用
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace PomodoroTimer.Services
{
    public class PomodoroService
    {
        private const string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string AppName = "PomodoroTimer";
        private const int WorkMinutes = 1;
        private const int ShortBreakMinutes = 1;
        private const int LongBreakMinutes = 30;
        private const int PomodorosUntilLongBreak = 4;
        private readonly string dataPath;

        // 公开工作时间常量供外部使用
        public static int DefaultWorkMinutes => WorkMinutes;

        public event EventHandler<TimeSpan> TimerTick;
        public event EventHandler BreakStarted;
        public event EventHandler WorkStarted;
        public event EventHandler PomodoroCompleted;  // 新增事件
        public event EventHandler BreakCompleted;  // 新增事件
        
        private System.Windows.Forms.Timer timer;
        private DateTime startTime;
        private int completedPomodoros;
        private PomodoroState currentState;

        public enum PomodoroState
        {
            Work,
            ShortBreak,
            LongBreak
        }

        public PomodoroService(string customDataPath = null)
        {
            dataPath = customDataPath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PomodoroTimer",
                "data.json"
            );
            
            var directory = Path.GetDirectoryName(dataPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // 初始化今天已完成的番茄钟数
            var existingData = LoadPomodoroData();
            var todayData = existingData.FirstOrDefault(d => d.Date.Date == DateTime.Today);
            completedPomodoros = todayData?.CompletedPomodoros ?? 0;
            
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            currentState = PomodoroState.Work;

            // 初始化时只更新显示，不触发计时器事件
            TimerTick?.Invoke(this, TimeSpan.FromMinutes(WorkMinutes));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - startTime;
            var targetDuration = GetTargetDuration();
            var remaining = targetDuration - elapsed;

            if (remaining.TotalSeconds <= 0)
            {
                HandlePomodoroComplete();
            }
            else
            {
                TimerTick?.Invoke(this, remaining);
            }
        }

        private TimeSpan GetTargetDuration()
        {
            return currentState switch
            {
                PomodoroState.Work => TimeSpan.FromMinutes(WorkMinutes),
                PomodoroState.ShortBreak => TimeSpan.FromMinutes(ShortBreakMinutes),
                PomodoroState.LongBreak => TimeSpan.FromMinutes(LongBreakMinutes),
                _ => TimeSpan.Zero
            };
        }

        private void HandlePomodoroComplete()
        {
            timer.Stop();
            if (currentState == PomodoroState.Work)
            {
                completedPomodoros++;
                SavePomodoroData();
                // 先触发完成事件，让界面有机会更新统计数据
                PomodoroCompleted?.Invoke(this, EventArgs.Empty);

                currentState = (completedPomodoros % PomodorosUntilLongBreak == 0) 
                    ? PomodoroState.LongBreak 
                    : PomodoroState.ShortBreak;
                
                // 更新显示新状态的时间
                TimerTick?.Invoke(this, GetTargetDuration());
                
                // 显示窗口并抖动
                ShowAndShakeWindow();
                
                BreakStarted?.Invoke(this, EventArgs.Empty);
                
                // 设置新的开始时间并启动计时器
                startTime = DateTime.Now;
                timer.Start();
            }
            else
            {
                currentState = PomodoroState.Work;
                // 停止计时器并重置状态
                timer.Stop();
                TimerTick?.Invoke(this, TimeSpan.FromMinutes(WorkMinutes));
                BreakCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        // New method to bring the main window to front and shake it
        private async void ShowAndShakeWindow()
        {
            var form = Application.OpenForms.Cast<Form>().FirstOrDefault();
            if (form != null)
            {
                // 开始时就设置置顶
                form.Invoke(new Action(() => {
                    form.TopMost = true;
                    form.BringToFront();
                    form.Activate();
                    form.WindowState = FormWindowState.Normal;
                    form.Show();
                }));

                var originalLocation = form.Location;
                int shakeAmplitude = 10;
                for (int i = 0; i < 10; i++)
                {
                    form.Invoke(new Action(() => {
                        form.Location = new Point(originalLocation.X + ((i % 2 == 0) ? shakeAmplitude : -shakeAmplitude), originalLocation.Y);
                        // 在抖动过程中持续保持置顶
                        form.TopMost = true;
                        form.BringToFront();
                    }));
                    await Task.Delay(50);
                }

                // 抖动结束后再次确保置顶
                form.Invoke(new Action(() => {
                    form.Location = originalLocation;
                    form.TopMost = true;
                    form.BringToFront();
                    form.Activate();
                }));
            }
        }

        public void Start()
        {
            startTime = DateTime.Now;
            currentState = PomodoroState.Work;
            WorkStarted?.Invoke(this, EventArgs.Empty);
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            // 重置计时器显示
            currentState = PomodoroState.Work;
            TimerTick?.Invoke(this, TimeSpan.FromMinutes(WorkMinutes));
        }

        public void SetAutoStart(bool enable)
        {
            using var key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            if (enable)
            {
                key?.SetValue(AppName, Application.ExecutablePath);
            }
            else
            {
                key?.DeleteValue(AppName, false);
            }
        }

        public bool GetAutoStartEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(StartupKey);
            return key?.GetValue(AppName) != null;
        }

        private void SavePomodoroData()
        {
            var data = new PomodoroData
            {
                Date = DateTime.Today,
                CompletedPomodoros = completedPomodoros,
                TotalFocusTime = TimeSpan.FromMinutes(completedPomodoros * WorkMinutes)
            };

            var existingData = LoadPomodoroData();
            var todayData = existingData.FirstOrDefault(d => d.Date.Date == DateTime.Today);
            
            if (todayData != null)
            {
                todayData.CompletedPomodoros = data.CompletedPomodoros;
                todayData.TotalFocusTime = data.TotalFocusTime;
            }
            else
            {
                existingData.Add(data);
            }

            File.WriteAllText(dataPath, JsonConvert.SerializeObject(existingData));
        }

        public List<PomodoroData> LoadPomodoroData()
        {
            if (!File.Exists(dataPath))
                return new List<PomodoroData>();

            var json = File.ReadAllText(dataPath);
            return JsonConvert.DeserializeObject<List<PomodoroData>>(json) ?? new List<PomodoroData>();
        }

        public TimeSpan GetCurrentBreakDuration()
        {
            return currentState switch
            {
                PomodoroState.ShortBreak => TimeSpan.FromMinutes(ShortBreakMinutes),
                PomodoroState.LongBreak => TimeSpan.FromMinutes(LongBreakMinutes),
                _ => TimeSpan.Zero
            };
        }

        public int GetTodayCompletedPomodoros()
        {
            var existingData = LoadPomodoroData();
            var todayData = existingData.FirstOrDefault(d => d.Date.Date == DateTime.Today);
            return todayData?.CompletedPomodoros ?? completedPomodoros;
        }

        [DllImport("user32.dll")]
        private static extern bool LockWorkStation();
    }
}
