using Xunit;
using PomodoroTimer.Services;
using System;
using System.IO;
using System.Threading;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PomodoroTimer.Tests
{
    public class PomodoroServiceTests : IDisposable
    {
        private readonly string testBasePath;

        public PomodoroServiceTests()
        {
            testBasePath = Path.Combine(Path.GetTempPath(), "PomodoroTimerTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(testBasePath);
        }

        public void Dispose()
        {
            if (Directory.Exists(testBasePath))
            {
                Directory.Delete(testBasePath, true);
            }
        }

        private string GetTestFilePath([CallerMemberName] string testName = "")
        {
            return Path.Combine(testBasePath, $"{testName}.json");
        }

        [Fact]
        public void Timer_Should_FireTickEvent()
        {
            // Arrange
            var service = new PomodoroService(GetTestFilePath());
            var tickFired = false;
            
            service.TimerTick += (s, remaining) => 
            {
                tickFired = true;
                Assert.True(remaining.TotalMinutes <= 25);
            };

            // Act
            service.Start();
            Thread.Sleep(2000); // wait for timer tick
            service.Stop();

            // Assert
            Assert.True(tickFired, "Timer tick event should have fired");
        }

        [Fact]
        public void CompletedPomodoro_Should_SaveToFile()
        {
            // Arrange
            var testPath = GetTestFilePath();
            var service = new PomodoroService(testPath);
            
            // Act
            var initialData = service.LoadPomodoroData();
            var initialCount = initialData.Count;

            // Simulate completing a pomodoro
            var field = typeof(PomodoroService).GetField("completedPomodoros", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(service, 1);

            // Use reflection to call private method
            var method = typeof(PomodoroService).GetMethod("SavePomodoroData", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(service, null);

            // Assert
            Assert.True(File.Exists(testPath), "Data file should be created");
            var savedData = service.LoadPomodoroData();
            Assert.True(savedData.Count > initialCount, "Data should be saved");
            Assert.Equal(1, savedData.Last().CompletedPomodoros);
        }

        [Fact]
        public void LongBreak_Should_StartAfterFourPomodoros()
        {
            // Arrange
            var service = new PomodoroService(GetTestFilePath());
            var longBreakStarted = false;
            
            service.BreakStarted += (s, e) => 
            {
                var stateField = typeof(PomodoroService).GetField("currentState", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var state = (PomodoroService.PomodoroState)stateField.GetValue(service);
                
                if (state == PomodoroService.PomodoroState.LongBreak)
                    longBreakStarted = true;
            };

            // Act
            // Simulate completing 3 pomodoros
            var completedField = typeof(PomodoroService).GetField("completedPomodoros", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            completedField.SetValue(service, 3);

            // Trigger completion of current pomodoro (this should be the 4th completed pomodoro and thus trigger a long break)
            var method = typeof(PomodoroService).GetMethod("HandlePomodoroComplete", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(service, null);

            // Assert
            Assert.True(longBreakStarted, "Long break should start after 4 pomodoros");
        }

        [Fact]
        public void AutoStart_Should_UpdateRegistryValue()
        {
            // Arrange
            var service = new PomodoroService(GetTestFilePath());

            try
            {
                // Act
                service.SetAutoStart(true);
                var enabled = service.GetAutoStartEnabled();
                
                service.SetAutoStart(false);
                var disabled = service.GetAutoStartEnabled();

                // Assert
                Assert.True(enabled);
                Assert.False(disabled);
            }
            finally
            {
                // Clean up registry
                service.SetAutoStart(false);
            }
        }
    }
}
