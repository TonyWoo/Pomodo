namespace PomodoroTimer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerLabel = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.stateLabel = new System.Windows.Forms.Label();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.statsLabel = new System.Windows.Forms.Label();
            
            // timerLabel
            this.timerLabel.AutoSize = true;
            this.timerLabel.Font = new System.Drawing.Font("Segoe UI", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.timerLabel.Location = new System.Drawing.Point(50, 20);
            this.timerLabel.Name = "timerLabel";
            this.timerLabel.Size = new System.Drawing.Size(200, 60);
            this.timerLabel.Text = "25:00";
            this.timerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // stateLabel
            this.stateLabel.AutoSize = true;
            this.stateLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.stateLabel.Location = new System.Drawing.Point(100, 90);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(100, 21);
            this.stateLabel.Text = "Work Time";
            this.stateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // startButton
            this.startButton.Location = new System.Drawing.Point(50, 130);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(90, 30);
            this.startButton.Text = "Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            
            // stopButton
            this.stopButton.Location = new System.Drawing.Point(160, 130);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(90, 30);
            this.stopButton.Text = "Stop";
            this.stopButton.Enabled = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            
            // chkAutoStart
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(50, 180);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(200, 19);
            this.chkAutoStart.Text = "Start with Windows";
            this.chkAutoStart.CheckedChanged += new System.EventHandler(this.chkAutoStart_CheckedChanged);

            // statsLabel
            this.statsLabel.AutoSize = true;
            this.statsLabel.Location = new System.Drawing.Point(50, 210);
            this.statsLabel.Name = "statsLabel";
            this.statsLabel.Size = new System.Drawing.Size(200, 15);
            this.statsLabel.Text = "Today: 0 pomodoros";
            this.statsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 250);
            this.Controls.Add(this.statsLabel);
            this.Controls.Add(this.chkAutoStart);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.stateLabel);
            this.Controls.Add(this.timerLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pomodoro Timer";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label timerLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.Label statsLabel;
    }
}
