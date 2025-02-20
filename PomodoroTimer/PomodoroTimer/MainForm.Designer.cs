﻿namespace PomodoroTimer
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
            this.timerLabel.AutoSize = false;
            this.timerLabel.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.timerLabel.Location = new System.Drawing.Point(0, 20);
            this.timerLabel.Name = "timerLabel";
            this.timerLabel.Size = new System.Drawing.Size(300, 70);
            this.timerLabel.Text = "25:00";
            this.timerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // stateLabel
            this.stateLabel.AutoSize = false;
            this.stateLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.stateLabel.Location = new System.Drawing.Point(0, 90);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(300, 30);
            this.stateLabel.Text = "Work Time";
            this.stateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // startButton
            this.startButton.Location = new System.Drawing.Point(40, 130);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(105, 40);
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = false;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            
            // stopButton
            this.stopButton.Location = new System.Drawing.Point(155, 130);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(105, 40);
            this.stopButton.Text = "Stop";
            this.stopButton.Enabled = false;
            this.stopButton.UseVisualStyleBackColor = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            
            // chkAutoStart
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(60, 180);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(180, 19);
            this.chkAutoStart.Text = "Start with Windows";
            this.chkAutoStart.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAutoStart.CheckedChanged += new System.EventHandler(this.chkAutoStart_CheckedChanged);

            // statsLabel
            this.statsLabel.AutoSize = false;
            this.statsLabel.Location = new System.Drawing.Point(0, 220);
            this.statsLabel.Name = "statsLabel";
            this.statsLabel.Size = new System.Drawing.Size(300, 20);
            this.statsLabel.Text = "Today: 0 pomodoros";
            this.statsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.statsLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            
            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 300);  // 增加窗口高度
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
