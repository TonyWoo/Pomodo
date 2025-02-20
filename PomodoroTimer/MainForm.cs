using PomodoroTimer.Services;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace PomodoroTimer
{
    public partial class Form1 : Form
    {
        private readonly PomodoroService pomodoroService;
        private NotifyIcon notifyIcon;
        private CheckBox chkTopMost;  // 添加置顶控件

        public Form1()
        {
            InitializeComponent();
            pomodoroService = new PomodoroService();          
 
            InitializeFormStyle();
            InitializeNotifyIcon();
            InitializeTopMostCheckBox();  // 初始化置顶控件
            SetupEventHandlers();
        }

        private void InitializeFormStyle()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new Size(300, 300);
            this.BackColor = Color.FromArgb(245, 245, 245);
            
            // 美化计时器标签
            timerLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            timerLabel.ForeColor = Color.FromArgb(64, 64, 64);
            
            // 美化状态标签
            stateLabel.Font = new Font("Segoe UI", 12);
            stateLabel.ForeColor = Color.FromArgb(100, 100, 100);
            
            // 美化按钮
            StyleButton(startButton);
            StyleButton(stopButton);
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
                Icon = this.Icon, // 使用相同的图标
                Visible = true,
                Text = "Pomodoro Timer"
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, (s, e) => { Show(); WindowState = FormWindowState.Normal; });
            contextMenu.Items.Add("View History", null, (s, e) => ShowHistory());
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

        private void InitializeTopMostCheckBox()
        {
            chkTopMost = new CheckBox
            {
                Text = "Always on Top",
                AutoSize = true,
                Location = new Point(60, 215),  // 调整位置
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            chkTopMost.CheckedChanged += ChkTopMost_CheckedChanged;
            Controls.Add(chkTopMost);
            
            // 调整 statsLabel 位置
            statsLabel.Location = new Point(statsLabel.Location.X, 250);
        }

        private void ChkTopMost_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = chkTopMost.Checked;
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
                notifyIcon.ShowBalloonTip(3000, "Pomodoro Timer", "Work time started!", ToolTipIcon.Info);
                // Ensure mouse restriction is removed during work time
                UnlockCursor();
            };

            pomodoroService.BreakStarted += (s, e) =>
            {
                stateLabel.Text = "Break Time";
                notifyIcon.ShowBalloonTip(3000, "Pomodoro Timer", "Break time! Screen will be locked.", ToolTipIcon.Info);
                this.Invoke(async () =>
                {
                    this.TopMost = true; // Set window as topmost
                    this.WindowState = FormWindowState.Normal; // Ensure the window is in normal state
                    this.Show();
                    this.BringToFront();
                    this.Activate();
                    await ShakeWindow();
                    this.TopMost = false; // Reset topmost property
                });
                LockCursor();
            };

            Load += (s, e) =>
            {
                chkAutoStart.Checked = pomodoroService.GetAutoStartEnabled();
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

        private void startButton_Click(object sender, EventArgs e)
        {
            pomodoroService.Start();
            startButton.Enabled = false;
            stopButton.Enabled = true;
            UnlockCursor(); // Call UnlockCursor after starting the work session
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            pomodoroService.Stop();
            startButton.Enabled = true;
            stopButton.Enabled = false;
            UnlockCursor();
        }

        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            pomodoroService.SetAutoStart(chkAutoStart.Checked);
        }

        // Restrict the mouse cursor to the bounds of the form's client area
        private void LockCursor()
        {
            System.Windows.Forms.Cursor.Clip = this.RectangleToScreen(this.ClientRectangle);
        }

        // Release the mouse cursor restriction
        private void UnlockCursor()
        {
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
                }));
                await Task.Delay(50);
            }
            this.Invoke(new Action(() => {
                this.Location = originalLocation;
            }));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle rect = this.ClientRectangle;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                Color.FromArgb(240, 248, 255),  // Alice Blue
                Color.FromArgb(230, 240, 250),  // 稍深的蓝色
                LinearGradientMode.Vertical))
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddRectangle(rect);
                    e.Graphics.FillPath(brush, path);
                }
            }
        }
    }
}
