namespace PomodoroTimer
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            timerLabel = new Label();
            startButton = new Button();
            stopButton = new Button();
            stateLabel = new Label();
            chkAutoStart = new CheckBox();
            topMostCheckBox = new CheckBox();
            statsLabel = new Label();
            SuspendLayout();
            // 
            // timerLabel
            // 
            timerLabel.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold);
            timerLabel.Location = new System.Drawing.Point(0, 20);
            timerLabel.Name = "timerLabel";
            timerLabel.Size = new System.Drawing.Size(300, 70);
            timerLabel.TabIndex = 5;
            timerLabel.Text = "25:00";
            timerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // startButton
            // 
            startButton.Location = new System.Drawing.Point(40, 130);
            startButton.Name = "startButton";
            startButton.Size = new System.Drawing.Size(105, 40);
            startButton.TabIndex = 3;
            startButton.Text = "Start";
            startButton.UseVisualStyleBackColor = false;
            startButton.Click += startButton_Click;
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new System.Drawing.Point(155, 130);
            stopButton.Name = "stopButton";
            stopButton.Size = new System.Drawing.Size(105, 40);
            stopButton.TabIndex = 2;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = false;
            stopButton.Click += stopButton_Click;
            // 
            // stateLabel
            // 
            stateLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            stateLabel.Location = new System.Drawing.Point(0, 90);
            stateLabel.Name = "stateLabel";
            stateLabel.Size = new System.Drawing.Size(300, 30);
            stateLabel.TabIndex = 4;
            stateLabel.Text = "Work Time";
            stateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkAutoStart
            // 
            chkAutoStart.AutoSize = true;
            chkAutoStart.Font = new System.Drawing.Font("Segoe UI", 9F);
            chkAutoStart.Location = new System.Drawing.Point(60, 180);
            chkAutoStart.Name = "chkAutoStart";
            chkAutoStart.Size = new System.Drawing.Size(128, 19);
            chkAutoStart.TabIndex = 1;
            chkAutoStart.Text = "Start with Windows";
            chkAutoStart.CheckedChanged += chkAutoStart_CheckedChanged;
            // 
            // topMostCheckBox
            // 
            topMostCheckBox.AutoSize = true;
            topMostCheckBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            topMostCheckBox.Location = new System.Drawing.Point(60, 200);
            topMostCheckBox.Name = "topMostCheckBox";
            topMostCheckBox.Size = new System.Drawing.Size(128, 19);
            topMostCheckBox.TabIndex = 6;
            topMostCheckBox.Text = "Always on Top";
            topMostCheckBox.CheckedChanged += topMostCheckBox_CheckedChanged;
            // 
            // statsLabel
            // 
            statsLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            statsLabel.Location = new System.Drawing.Point(0, 240);
            statsLabel.Name = "statsLabel";
            statsLabel.Size = new System.Drawing.Size(300, 20);
            statsLabel.TabIndex = 0;
            statsLabel.Text = "Today: 0 pomodoros";
            statsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(300, 300);
            Controls.Add(topMostCheckBox);
            Controls.Add(statsLabel);
            Controls.Add(chkAutoStart);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(stateLabel);
            Controls.Add(timerLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Pomodoro Timer";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label timerLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.Label statsLabel;
        private System.Windows.Forms.CheckBox topMostCheckBox;
    }
}
