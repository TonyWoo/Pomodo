using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using PomodoroTimer.Models;
using System.Windows.Forms;  // 添加这个引用

namespace PomodoroTimer.Services
{
    public class PomodoroService
    {
        private const string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string AppName = "PomodoroTimer";
        private const int WorkMinutes = 25;
        private const int ShortBreakMinutes = 5;
        private const int LongBreakMinutes = 30;
        private const int PomodorosUntilLongBreak = 4;
        private readonly string dataPath;

        public event EventHandler<TimeSpan> TimerTick;
        public event EventHandler BreakStarted;
        public event EventHandler WorkStarted;
        
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
                
                currentState = (completedPomodoros % PomodorosUntilLongBreak == 0) 
                    ? PomodoroState.LongBreak 
                    : PomodoroState.ShortBreak;
                
                BreakStarted?.Invoke(this, EventArgs.Empty);
                LockWorkStation();
            }
            else
            {
                currentState = PomodoroState.Work;
                WorkStarted?.Invoke(this, EventArgs.Empty);
            }
            startTime = DateTime.Now;
            timer.Start();
        }

        public void Start()
        {
            startTime = DateTime.Now;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            // 重置计时器显示
            currentState = PomodoroState.Work;
            TimerTick?.Invoke(this, TimeSpan.FromMinutes(WorkMinutes));
            completedPomodoros = 0;
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

        [DllImport("user32.dll")]
        private static extern bool LockWorkStation();
    }
}