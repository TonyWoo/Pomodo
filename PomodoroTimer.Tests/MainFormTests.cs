using System;
using System.Drawing;
using System.Windows.Forms;
using Xunit;
using PomodoroTimer;

namespace PomodoroTimer.Tests
{
    // A testable subclass of MainForm that exposes the OnPaint method
    public class TestableMainForm : MainForm
    {
        public void PublicOnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }

    public class MainFormTests
    {
        [STAThread]
        [Fact]
        public void OnPaint_Should_NotThrow_When_ClientRectangleIsZero()
        {
            // Arrange
            var form = new TestableMainForm();
            form.ClientSize = new Size(0, 0);
            using (var bitmap = new Bitmap(1, 1))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var paintEvent = new PaintEventArgs(graphics, new Rectangle(0, 0, 0, 0));

                // Act
                var exception = Record.Exception(() => form.PublicOnPaint(paintEvent));

                // Assert
                Assert.Null(exception);
            }
        }
    }
}
