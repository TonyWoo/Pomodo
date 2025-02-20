using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Versioning;

namespace PomodoroTimer
{
    [SupportedOSPlatform("windows")]
    public class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "About Pomodoro Timer";
            this.Size = new Size(300, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            TableLayoutPanel tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 1,
                RowCount = 4
            };

            Label titleLabel = new Label
            {
                Text = "Pomodoro Timer",
                Font = new Font(Font.FontFamily, 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            Label authorLabel = new Label
            {
                Text = "Author: Tony Wu",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            Label versionLabel = new Label
            {
                Text = "Version: v1.0",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            Label dateLabel = new Label
            {
                Text = "Release Date: 2025/2/25",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            tableLayout.Controls.Add(titleLabel, 0, 0);
            tableLayout.Controls.Add(authorLabel, 0, 1);
            tableLayout.Controls.Add(versionLabel, 0, 2);
            tableLayout.Controls.Add(dateLabel, 0, 3);

            this.Controls.Add(tableLayout);
        }
    }
}