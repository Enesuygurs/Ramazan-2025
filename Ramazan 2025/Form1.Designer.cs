namespace Ramazan_2025
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
            lblTime1 = new Label();
            lblTime2 = new Label();
            lblTime3 = new Label();
            lblTime4 = new Label();
            lblTime5 = new Label();
            Title = new Label();
            btnClose = new Label();
            btnSettings = new Label();
            lblKalanZaman = new Label();
            timerKalanSure = new System.Windows.Forms.Timer(components);
            reminderNotification = new NotifyIcon(components);
            SuspendLayout();
            // 
            // lblTime1
            // 
            lblTime1.AutoSize = true;
            lblTime1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTime1.ForeColor = Color.WhiteSmoke;
            lblTime1.Location = new Point(29, 102);
            lblTime1.Name = "lblTime1";
            lblTime1.Size = new Size(51, 21);
            lblTime1.TabIndex = 0;
            lblTime1.Text = "label1";
            // 
            // lblTime2
            // 
            lblTime2.AutoSize = true;
            lblTime2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTime2.ForeColor = Color.WhiteSmoke;
            lblTime2.Location = new Point(29, 132);
            lblTime2.Name = "lblTime2";
            lblTime2.Size = new Size(54, 21);
            lblTime2.TabIndex = 1;
            lblTime2.Text = "label2";
            // 
            // lblTime3
            // 
            lblTime3.AutoSize = true;
            lblTime3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTime3.ForeColor = Color.WhiteSmoke;
            lblTime3.Location = new Point(29, 162);
            lblTime3.Name = "lblTime3";
            lblTime3.Size = new Size(54, 21);
            lblTime3.TabIndex = 2;
            lblTime3.Text = "label3";
            // 
            // lblTime4
            // 
            lblTime4.AutoSize = true;
            lblTime4.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTime4.ForeColor = Color.WhiteSmoke;
            lblTime4.Location = new Point(29, 192);
            lblTime4.Name = "lblTime4";
            lblTime4.Size = new Size(54, 21);
            lblTime4.TabIndex = 3;
            lblTime4.Text = "label4";
            // 
            // lblTime5
            // 
            lblTime5.AutoSize = true;
            lblTime5.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTime5.ForeColor = Color.WhiteSmoke;
            lblTime5.Location = new Point(29, 222);
            lblTime5.Name = "lblTime5";
            lblTime5.Size = new Size(54, 21);
            lblTime5.TabIndex = 4;
            lblTime5.Text = "label5";
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
            btnClose.Location = new Point(193, 3);
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
            btnSettings.Location = new Point(173, 2);
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
            lblKalanZaman.Location = new Point(50, 41);
            lblKalanZaman.Name = "lblKalanZaman";
            lblKalanZaman.Size = new Size(120, 42);
            lblKalanZaman.TabIndex = 8;
            lblKalanZaman.Text = "Kalan Süre\r\n08:02:25";
            lblKalanZaman.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // timerKalanSure
            // 
            timerKalanSure.Enabled = true;
            timerKalanSure.Interval = 1000;
            timerKalanSure.Tick += timerKalanSure_Tick;
            // 
            // reminderNotification
            // 
            reminderNotification.Text = "Ramazan 2025";
            reminderNotification.Visible = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(220, 270);
            Controls.Add(lblKalanZaman);
            Controls.Add(btnSettings);
            Controls.Add(btnClose);
            Controls.Add(Title);
            Controls.Add(lblTime5);
            Controls.Add(lblTime4);
            Controls.Add(lblTime3);
            Controls.Add(lblTime2);
            Controls.Add(lblTime1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            Opacity = 0.9D;
            ShowInTaskbar = false;
            Text = "Ramazan 2025";
            Load += Form1_Load_1;
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
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
        private System.Windows.Forms.Timer timerKalanSure;
        private NotifyIcon reminderNotification;
    }
}
