using System;

namespace PomodoroTimer.Models
{
    public class PomodoroData
    {
        public DateTime Date { get; set; }
        public int CompletedPomodoros { get; set; }
        public TimeSpan TotalFocusTime { get; set; }
    }
}