using PomodoroTimer.Services;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Versioning;

namespace PomodoroTimer
{
    [SupportedOSPlatform("windows")]
    public partial class MainForm : Form
    {
        private readonly PomodoroService pomodoroService;
        private NotifyIcon notifyIcon = null!;
        private System.Windows.Forms.Timer cursorCheckTimer = null!;

        public MainForm()
        {
            InitializeComponent();
            pomodoroService = new PomodoroService();          

            InitializeFormStyle();
            InitializeNotifyIcon();
            SetupEventHandlers();
        }

        private void InitializeFormStyle()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new Size(300, 300);
            this.BackColor = Color.FromArgb(245, 245, 245);
            
            // Beautify timer label
            timerLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            timerLabel.ForeColor = Color.FromArgb(64, 64, 64);
            
            // Beautify state label
            stateLabel.Font = new Font("Segoe UI", 12);
            stateLabel.ForeColor = Color.FromArgb(100, 100, 100);
            
            // Set initial state and time
            stateLabel.Text = "Work Time";
            timerLabel.Text = $"{PomodoroService.DefaultWorkMinutes:D2}:00";
            
            // Beautify buttons
            StyleButton(startButton);
            StyleButton(stopButton);

            // Initialize mouse position check timer
            cursorCheckTimer = new System.Windows.Forms.Timer();
            cursorCheckTimer.Interval = 100; // Check every 100ms
            cursorCheckTimer.Tick += CursorCheckTimer_Tick;
        }

        private void StyleButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = Color.FromArgb(0, 120, 215);
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 10);
            button.Padding = new Padding(10, 5, 10, 5);
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.BorderColor = Color.FromArgb(0, 90, 180);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 140, 230);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 100, 190);
            button.Cursor = Cursors.Hand;
            button.Region = new Region(RoundedRect(new RectangleF(0, 0, button.Width, button.Height), 8));
        }

        private GraphicsPath RoundedRect(RectangleF bounds, float radius)
        {
            float diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Width - diameter + bounds.X, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Width - diameter + bounds.X, bounds.Height - diameter + bounds.Y, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Height - diameter + bounds.Y, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = this.Icon, // Use the same icon
                Visible = true,
                Text = "Pomodoro Timer"
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, (s, e) => { Show(); WindowState = FormWindowState.Normal; });
            contextMenu.Items.Add("View History", null, (s, e) => ShowHistory());
            contextMenu.Items.Add("About", null, (s, e) => ShowAboutDialog());
            contextMenu.Items.Add("Exit", null, (s, e) => Application.Exit());
            notifyIcon.ContextMenuStrip = contextMenu;

            notifyIcon.DoubleClick += (s, e) => { Show(); WindowState = FormWindowState.Normal; };
        }

        private void ShowHistory()
        {
            var historyData = pomodoroService.LoadPomodoroData();
            var historyForm = new HistoryForm(historyData);
            historyForm.ShowDialog();
        }

        private void ShowAboutDialog()
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog(this);
        }

        private void topMostCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = topMostCheckBox.Checked;
        }

        private void SetupEventHandlers()
        {
            pomodoroService.TimerTick += (s, remaining) =>
            {
                timerLabel.Text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            };

            pomodoroService.WorkStarted += (s, e) =>
            {
                stateLabel.Text = "Work Time";
                timerLabel.Text = $"{PomodoroService.DefaultWorkMinutes:D2}:00";
                startButton.Enabled = false;
                stopButton.Enabled = true;
                notifyIcon.ShowBalloonTip(3000, "Pomodoro Timer", "Work time started!", ToolTipIcon.Info);
                this.TopMost = false;
                UnlockCursor();
                // Make sure to stop the timer when work starts
                cursorCheckTimer.Stop();
            };

            pomodoroService.BreakStarted += (s, e) =>
            {
                var breakDuration = pomodoroService.GetCurrentBreakDuration();
                stateLabel.Text = "Break Time";
                timerLabel.Text = $"{breakDuration.Minutes:D2}:{breakDuration.Seconds:D2}";
                startButton.Enabled = false;
                stopButton.Enabled = true;
                notifyIcon.ShowBalloonTip(3000, "Pomodoro Timer", "Break time! Screen will be locked.", ToolTipIcon.Info);
                
                this.Invoke(async () =>
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.TopMost = true;
                    this.BringToFront();
                    this.Activate();
                    this.Focus();
                    
                    await Task.Delay(100);
                    this.TopMost = true;
                    this.BringToFront();
                    
                    await ShakeWindow();
                    
                    this.TopMost = true;
                    this.BringToFront();
                    this.Activate();
                    
                    LockCursor();
                });
            };

            pomodoroService.PomodoroCompleted += (s, e) =>
            {
                startButton.Enabled = true;
                stopButton.Enabled = false;
                UpdateTodayStats();
            };

            pomodoroService.BreakCompleted += (s, e) =>
            {
                startButton.Enabled = true;
                stopButton.Enabled = false;
                UnlockCursor();
                cursorCheckTimer.Stop();
                this.TopMost = topMostCheckBox.Checked;
            };

            Load += (s, e) =>
            {
                chkAutoStart.Checked = pomodoroService.GetAutoStartEnabled();
                UpdateTodayStats();
            };

            FormClosing += (s, e) =>
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    Hide();
                }
            };
        }

        private void UpdateTodayStats()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateTodayStats));
                return;
            }

            var todayPomodoros = pomodoroService.GetTodayCompletedPomodoros();
            statsLabel.Text = $"Today: {todayPomodoros} pomodoros";
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            pomodoroService.Start();
            startButton.Enabled = false;
            stopButton.Enabled = true;
            UnlockCursor(); // Call UnlockCursor after starting the work session
            // Make sure to stop the timer when work starts
            cursorCheckTimer.Stop();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            pomodoroService.Stop();
            startButton.Enabled = true;
            stopButton.Enabled = false;
            UnlockCursor();
            // Make sure to stop the timer when stopped
            cursorCheckTimer.Stop();
        }

        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            pomodoroService.SetAutoStart(chkAutoStart.Checked);
        }

        // Timed check when the mouse is locked
        private void CursorCheckTimer_Tick(object? sender, EventArgs e)
        {
            if (!this.Visible || this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }

            // Get the actual position of the window client area on the screen
            Point clientOrigin = this.PointToScreen(Point.Empty);
            Rectangle screenRect = new Rectangle(
                clientOrigin.X,
                clientOrigin.Y,
                this.ClientRectangle.Width,
                this.ClientRectangle.Height
            );

            // If the mouse is outside the window, force it back to the center of the window
            Point currentPos = System.Windows.Forms.Cursor.Position;
            if (!screenRect.Contains(currentPos))
            {
                Point centerPoint = new Point(
                    screenRect.X + screenRect.Width / 2,
                    screenRect.Y + screenRect.Height / 2
                );
                System.Windows.Forms.Cursor.Position = centerPoint;
            }

            // Ensure the window is always on top
            this.TopMost = true;
            this.BringToFront();
            this.Activate();
        }

        // Restrict the mouse cursor to the bounds of the form's client area
        private void LockCursor()
        {
            // Get the actual position of the window client area on the screen
            Point clientOrigin = this.PointToScreen(Point.Empty);
            Rectangle screenRect = new Rectangle(
                clientOrigin.X,
                clientOrigin.Y,
                this.ClientRectangle.Width,
                this.ClientRectangle.Height
            );
            
            // Set the cursor restriction area to the client area
            System.Windows.Forms.Cursor.Clip = screenRect;
            
            // Center the mouse to the center of the window client area
            Point centerPoint = new Point(
                screenRect.X + screenRect.Width / 2,
                screenRect.Y + screenRect.Height / 2
            );
            System.Windows.Forms.Cursor.Position = centerPoint;

            // Start the mouse position check timer
            cursorCheckTimer.Start();
        }

        // Release the mouse cursor restriction
        private void UnlockCursor()
        {
            cursorCheckTimer.Stop();
            System.Windows.Forms.Cursor.Clip = System.Drawing.Rectangle.Empty;
        }

        private async Task ShakeWindow()
        {
            var originalLocation = this.Location;
            int shakeAmplitude = 10;
            for (int i = 0; i < 10; i++)
            {
                this.Invoke(new Action(() => {
                    this.Location = new Point(originalLocation.X + ((i % 2 == 0) ? shakeAmplitude : -shakeAmplitude), originalLocation.Y);
                    // Keep the window on top during the shake
                    this.TopMost = true;
                    this.BringToFront();
                }));
                await Task.Delay(50);
            }
            this.Invoke(new Action(() => {
                this.Location = originalLocation;
                // Ensure the window is still on top after the shake
                this.TopMost = true;
                this.BringToFront();
                this.Activate();
            }));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle rect = this.ClientRectangle;
            if(rect.Width == 0 || rect.Height == 0)
            {
                return;
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                Color.FromArgb(240, 248, 255),  // Alice Blue
                Color.FromArgb(230, 240, 250),  // Slightly darker blue
                LinearGradientMode.Vertical))
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddRectangle(rect);
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            // Ensure timer resources are cleaned up
            if (cursorCheckTimer != null)
            {
                cursorCheckTimer.Stop();
                cursorCheckTimer.Dispose();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }
    }
}
