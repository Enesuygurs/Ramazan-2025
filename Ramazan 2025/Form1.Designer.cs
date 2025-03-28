﻿namespace Ramazan_2025
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            lblTime1 = new Label();
            lblTime2 = new Label();
            lblTime3 = new Label();
            lblTime4 = new Label();
            lblTime5 = new Label();
            Title = new Label();
            btnClose = new Label();
            btnSettings = new Label();
            lblKalanZaman = new Label();
            timerRemainingTime = new System.Windows.Forms.Timer(components);
            reminderNotification = new NotifyIcon(components);
            taskbarMenu = new ContextMenuStrip(components);
            exitToolStripMenuItem = new ToolStripMenuItem();
            lblChangeSize = new Label();
            lblRamadanDay = new Label();
            lblCity = new Label();
            pnlButtons = new Panel();
            taskbarMenu.SuspendLayout();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            // 
            // lblTime1
            // 
            lblTime1.AutoSize = true;
            lblTime1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTime1.ForeColor = Color.WhiteSmoke;
            lblTime1.Location = new Point(29, 144);
            lblTime1.Name = "lblTime1";
            lblTime1.Size = new Size(61, 21);
            lblTime1.TabIndex = 0;
            lblTime1.Text = "İmsak: ";
            // 
            // lblTime2
            // 
            lblTime2.AutoSize = true;
            lblTime2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTime2.ForeColor = Color.WhiteSmoke;
            lblTime2.Location = new Point(29, 174);
            lblTime2.Name = "lblTime2";
            lblTime2.Size = new Size(63, 21);
            lblTime2.TabIndex = 1;
            lblTime2.Text = "Güneş: ";
            // 
            // lblTime3
            // 
            lblTime3.AutoSize = true;
            lblTime3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTime3.ForeColor = Color.WhiteSmoke;
            lblTime3.Location = new Point(29, 204);
            lblTime3.Name = "lblTime3";
            lblTime3.Size = new Size(53, 21);
            lblTime3.TabIndex = 2;
            lblTime3.Text = "Öğle: ";
            // 
            // lblTime4
            // 
            lblTime4.AutoSize = true;
            lblTime4.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTime4.ForeColor = Color.WhiteSmoke;
            lblTime4.Location = new Point(29, 234);
            lblTime4.Name = "lblTime4";
            lblTime4.Size = new Size(58, 21);
            lblTime4.TabIndex = 3;
            lblTime4.Text = "İkindi: ";
            // 
            // lblTime5
            // 
            lblTime5.AutoSize = true;
            lblTime5.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTime5.ForeColor = Color.WhiteSmoke;
            lblTime5.Location = new Point(29, 264);
            lblTime5.Name = "lblTime5";
            lblTime5.Size = new Size(51, 21);
            lblTime5.TabIndex = 4;
            lblTime5.Text = "Yatsı: ";
            // 
            // Title
            // 
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            Title.ForeColor = Color.White;
            Title.Location = new Point(5, 6);
            Title.Name = "Title";
            Title.Size = new Size(85, 15);
            Title.TabIndex = 5;
            Title.Text = "İmsakiye 2025";
            Title.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            btnClose.AutoSize = true;
            btnClose.Font = new Font("Webdings", 11.25F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(44, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(25, 20);
            btnClose.TabIndex = 6;
            btnClose.Text = "r";
            btnClose.Click += btnClose_Click;
            // 
            // btnSettings
            // 
            btnSettings.AutoSize = true;
            btnSettings.Font = new Font("Segoe UI Symbol", 12F, FontStyle.Bold);
            btnSettings.ForeColor = Color.White;
            btnSettings.Location = new Point(24, 2);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(24, 21);
            btnSettings.TabIndex = 7;
            btnSettings.Text = "⚙";
            btnSettings.Click += btnSettings_Click;
            // 
            // lblKalanZaman
            // 
            lblKalanZaman.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblKalanZaman.ForeColor = Color.WhiteSmoke;
            lblKalanZaman.Location = new Point(31, 84);
            lblKalanZaman.Name = "lblKalanZaman";
            lblKalanZaman.Size = new Size(158, 52);
            lblKalanZaman.TabIndex = 8;
            lblKalanZaman.Text = "Kalan Süre\r\n08:02:25";
            lblKalanZaman.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // timerRemainingTime
            // 
            timerRemainingTime.Interval = 1000;
            timerRemainingTime.Tick += timerRemainingTime_Tick;
            // 
            // reminderNotification
            // 
            reminderNotification.ContextMenuStrip = taskbarMenu;
            reminderNotification.Icon = (Icon)resources.GetObject("reminderNotification.Icon");
            reminderNotification.Text = "Ramazan 2025";
            reminderNotification.Visible = true;
            // 
            // taskbarMenu
            // 
            taskbarMenu.Items.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            taskbarMenu.Name = "taskbarMenu";
            taskbarMenu.Size = new Size(100, 26);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(99, 22);
            exitToolStripMenuItem.Text = "Çıkış";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // lblChangeSize
            // 
            lblChangeSize.AutoSize = true;
            lblChangeSize.Font = new Font("Segoe UI Symbol", 11F, FontStyle.Bold);
            lblChangeSize.ForeColor = Color.White;
            lblChangeSize.Location = new Point(2, 3);
            lblChangeSize.Name = "lblChangeSize";
            lblChangeSize.Size = new Size(25, 20);
            lblChangeSize.TabIndex = 9;
            lblChangeSize.Text = "⏶";
            lblChangeSize.Click += lblChangeSize_Click;
            // 
            // lblRamadanDay
            // 
            lblRamadanDay.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lblRamadanDay.ForeColor = Color.WhiteSmoke;
            lblRamadanDay.Location = new Point(50, 30);
            lblRamadanDay.Name = "lblRamadanDay";
            lblRamadanDay.Size = new Size(120, 32);
            lblRamadanDay.TabIndex = 10;
            lblRamadanDay.Text = "24. Gün";
            lblRamadanDay.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblCity
            // 
            lblCity.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblCity.ForeColor = Color.OrangeRed;
            lblCity.Location = new Point(50, 62);
            lblCity.Name = "lblCity";
            lblCity.Size = new Size(120, 22);
            lblCity.TabIndex = 11;
            lblCity.Text = "İstanbul";
            lblCity.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlButtons
            // 
            pnlButtons.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlButtons.BackColor = Color.Black;
            pnlButtons.Controls.Add(btnClose);
            pnlButtons.Controls.Add(btnSettings);
            pnlButtons.Controls.Add(lblChangeSize);
            pnlButtons.Location = new Point(149, 0);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(75, 25);
            pnlButtons.TabIndex = 12;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(220, 310);
            Controls.Add(lblCity);
            Controls.Add(lblRamadanDay);
            Controls.Add(lblKalanZaman);
            Controls.Add(Title);
            Controls.Add(lblTime5);
            Controls.Add(lblTime4);
            Controls.Add(lblTime3);
            Controls.Add(lblTime2);
            Controls.Add(lblTime1);
            Controls.Add(pnlButtons);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(225, 320);
            MinimumSize = new Size(220, 310);
            Name = "Form1";
            Opacity = 0.8D;
            ShowInTaskbar = false;
            Text = "Ramazan 2025";
            Load += Form1_Load;
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
            Resize += Form1_Resize;
            taskbarMenu.ResumeLayout(false);
            pnlButtons.ResumeLayout(false);
            pnlButtons.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTime1;
        private Label lblTime2;
        private Label lblTime3;
        private Label lblTime4;
        private Label lblTime5;
        private Label Title;
        private Label btnClose;
        private Label btnSettings;
        private Label lblKalanZaman;
        private System.Windows.Forms.Timer timerRemainingTime;
        private NotifyIcon reminderNotification;
        private Label lblChangeSize;
        private Label lblRamadanDay;
        private ContextMenuStrip taskbarMenu;
        private ToolStripMenuItem exitToolStripMenuItem;
        private Label lblCity;
        private Panel pnlButtons;
    }
}
