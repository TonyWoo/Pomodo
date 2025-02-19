using PomodoroTimer.Services;
using System.Windows.Forms;

namespace PomodoroTimer
{
    public partial class Form1 : Form
    {
        private readonly PomodoroService pomodoroService;
        private NotifyIcon notifyIcon;

        public Form1()
        {
            InitializeComponent();
            pomodoroService = new PomodoroService();
            InitializeNotifyIcon();
            SetupEventHandlers();
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = "Pomodoro Timer"
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, (s, e) => { Show(); WindowState = FormWindowState.Normal; });
            contextMenu.Items.Add("Exit", null, (s, e) => Application.Exit());
            notifyIcon.ContextMenuStrip = contextMenu;

            notifyIcon.DoubleClick += (s, e) => { Show(); WindowState = FormWindowState.Normal; };
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
            };

            pomodoroService.BreakStarted += (s, e) =>
            {
                stateLabel.Text = "Break Time";
                notifyIcon.ShowBalloonTip(3000, "Pomodoro Timer", "Break time! Screen will be locked.", ToolTipIcon.Info);
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
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            pomodoroService.Stop();
            startButton.Enabled = true;
            stopButton.Enabled = false;
        }

        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            pomodoroService.SetAutoStart(chkAutoStart.Checked);
        }
    }
}
