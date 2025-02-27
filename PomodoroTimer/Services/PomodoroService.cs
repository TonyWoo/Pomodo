using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using PomodoroTimer.Models;
using System.Windows.Forms;
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

        public static int DefaultWorkMinutes => WorkMinutes;

        public event EventHandler<TimeSpan> TimerTick;
        public event EventHandler BreakStarted;
        public event EventHandler WorkStarted;
        public event EventHandler PomodoroCompleted;  
        public event EventHandler BreakCompleted;
        
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
            
            // Initialize the number of completed pomodoros today
            var existingData = LoadPomodoroData();
            var todayData = existingData.FirstOrDefault(d => d.Date.Date == DateTime.Today);
            completedPomodoros = todayData?.CompletedPomodoros ?? 0;
            
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            currentState = PomodoroState.Work;

            // Update display only at initialization, do not trigger timer event
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
                // Trigger completion event first to let the UI update statistics
                PomodoroCompleted?.Invoke(this, EventArgs.Empty);
                currentState = (completedPomodoros % PomodorosUntilLongBreak == 0) 
                    ? PomodoroState.LongBreak 
                    : PomodoroState.ShortBreak;
                
                // Update time display for new state
                TimerTick?.Invoke(this, GetTargetDuration());
                
                // Show window and shake
                //ShowAndShakeWindow();
                
                BreakStarted?.Invoke(this, EventArgs.Empty);
                
                // Set new start time and start timer
                startTime = DateTime.Now;
                timer.Start();
            }
            else
            {
                currentState = PomodoroState.Work;
                // Stop timer and reset state
                timer.Stop();
                TimerTick?.Invoke(this, TimeSpan.FromMinutes(WorkMinutes));
                BreakCompleted?.Invoke(this, EventArgs.Empty);
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
            // Reset timer display
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
