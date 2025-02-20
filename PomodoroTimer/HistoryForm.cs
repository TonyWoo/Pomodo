using System.Windows.Forms;
using System.Drawing;
using PomodoroTimer.Models;

namespace PomodoroTimer
{
    public class HistoryForm : Form
    {
        private ListView listView;

        public HistoryForm(List<PomodoroData> historyData)
        {
            InitializeComponent();
            LoadHistoryData(historyData);
        }

        private void InitializeComponent()
        {
            this.Text = "Pomodoro History";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Dock = DockStyle.Fill
            };

            listView.Columns.Add("Date", 200);
            listView.Columns.Add("Pomodoros", 100);
            listView.Columns.Add("Focus Time", 100);

            this.Controls.Add(listView);
        }

        private void LoadHistoryData(List<PomodoroData> historyData)
        {
            foreach (var data in historyData.OrderByDescending(d => d.Date))
            {
                var item = new ListViewItem(data.Date.ToShortDateString());
                item.SubItems.Add(data.CompletedPomodoros.ToString());
                item.SubItems.Add($"{data.TotalFocusTime.TotalHours:F1} hrs");
                listView.Items.Add(item);
            }
        }
    }
}