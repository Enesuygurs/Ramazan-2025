using System.Runtime.InteropServices;

namespace Ramazan_2025 {
    public partial class Form1 : Form {

        #region Fields
        private string _fajr, _dhuhr, _asr, _maghrib, _isha;
        private string _tomorrowFajr;
        private DateTime _sonKontrolEdilenGun = DateTime.Now.Date;
        private System.Windows.Forms.Label _aktifLabel = null;
        private PrayerTimes _prayerTimes = new PrayerTimes();
        #endregion

        #region Components
        // Pencereyi en alta almak için gerekli olan sabitler
        private const int HWND_BOTTOM = 1;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        #endregion

        public Form1() {
            InitializeComponent();
            SetWindowPos(this.Handle, (IntPtr)HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW); // Widget olarak ayarla
            SetFormPosition(); // Formun konumunu ayarlayan metod
        }

        private async void Form1_Load_1(object sender, EventArgs e) {
            //Properties.Settings.Default.Reset();
            //Properties.Settings.Default.Save(); 
            await GetPrayerTimes();
        }

        #region Ramadan Timetable Operations 
        public async Task GetPrayerTimes() {
            try {
                string selectedCity = Properties.Settings.Default.SelectedCity ?? "Ýstanbul";
                var result = await _prayerTimes.GetPrayerTimes(selectedCity);

                if (new[] { result.Fajr, result.Dhuhr, result.Asr, result.Maghrib, result.Isha, result.TomorrowFajr }.Any(string.IsNullOrEmpty)) {
                    MessageBox.Show("Namaz vakitleri eksik veya hatalý geldi!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Baþarýyla çekilen veriyi atama
                _fajr = result.Fajr;
                _dhuhr = result.Dhuhr;
                _asr = result.Asr;
                _maghrib = result.Maghrib;
                _isha = result.Isha;
                _tomorrowFajr = result.TomorrowFajr;

                // Arayüzü güncelleme
                lblTime1.Text = $"Ýmsak: {_fajr}";
                lblTime2.Text = $"Öðle: {_dhuhr}";
                lblTime3.Text = $"Ýkindi: {_asr}";
                lblTime4.Text = $"Akþam: {_maghrib}";
                lblTime5.Text = $"Yatsý: {_isha}";
                lblRamadanDay.Text = $"{result.Day}. Gün";

                if (!timerKalanSure.Enabled) timerKalanSure.Enabled = true;
            } catch (Exception ex) {
                MessageBox.Show($"Namaz vakitleri alýnýrken bir hata oluþtu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void timerKalanSure_Tick(object sender, EventArgs e) {
            try {
                DateTime simdikiZaman = DateTime.Now;
                DateTime today = simdikiZaman.Date;

                // Eðer gün deðiþtiyse, namaz vakitlerini güncelle
                if (_sonKontrolEdilenGun != today) {
                    await GetPrayerTimes(); // Yeni namaz vakitlerini çek
                    _sonKontrolEdilenGun = today; // Son kontrol edilen günü güncelle
                }

                DateTime sahurVakti, iftarVakti, yarinSahurVakti;
                System.Windows.Forms.Label yeniAktifLabel = null;

                try {
                    // Null kontrolü ekleyerek hatalarý önlüyoruz
                    if (string.IsNullOrEmpty(_fajr) || string.IsNullOrEmpty(_maghrib) || string.IsNullOrEmpty(_tomorrowFajr)) {
                        MessageBox.Show("Namaz vakitleri yüklenemedi. Lütfen internet baðlantýnýzý kontrol edin.");
                    }

                    sahurVakti = DateTime.Parse(_fajr);
                    iftarVakti = DateTime.Parse(_maghrib);
                    yarinSahurVakti = DateTime.Parse(_tomorrowFajr);

                    // Bugüne eklenmiþ saatleri oluþtur
                    sahurVakti = today.Add(sahurVakti.TimeOfDay);
                    iftarVakti = today.Add(iftarVakti.TimeOfDay);
                    yarinSahurVakti = today.AddDays(1).Add(yarinSahurVakti.TimeOfDay);
                } catch (Exception ex) {
                    MessageBox.Show($"Namaz vakitleri alýnýrken hata oluþtu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Hata olursa iþlemi devam ettirme
                }

                if (simdikiZaman < sahurVakti) {
                    TimeSpan kalanSure = sahurVakti - simdikiZaman;
                    lblKalanZaman.Text = $"Kalan Süre\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                    yeniAktifLabel = lblTime1;
                    if (Properties.Settings.Default.reminder && kalanSure.Hours == 0 && kalanSure.Minutes == 15 && kalanSure.Seconds == 0) ShowNotification("Sahur Vakti Yaklaþýyor!", "Sahura 15 dakika kaldý.");
                } else if (simdikiZaman > sahurVakti && simdikiZaman < iftarVakti) {
                    TimeSpan kalanSure = iftarVakti - simdikiZaman;
                    lblKalanZaman.Text = $"Kalan Süre\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                    yeniAktifLabel = lblTime4;
                    if (Properties.Settings.Default.reminder && kalanSure.Hours == 0 && kalanSure.Minutes == 15 && kalanSure.Seconds == 0) ShowNotification("Ýftar Vakti Yaklaþýyor!", "Ýftara 15 dakika kaldý.");
                } else {
                    TimeSpan kalanSure = yarinSahurVakti - simdikiZaman;
                    lblKalanZaman.Text = $"Kalan Süre\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                    yeniAktifLabel = lblTime1;
                }

                if (_aktifLabel != yeniAktifLabel) {
                    if (_aktifLabel != null) _aktifLabel.ForeColor = Color.WhiteSmoke;
                    if (yeniAktifLabel != null) {
                        yeniAktifLabel.ForeColor = Color.Red;
                        _aktifLabel = yeniAktifLabel;
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show($"Bir hata oluþtu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Notifications
        private void ShowNotification(string title, string message) {
            reminderNotification.Icon = SystemIcons.Information;
            reminderNotification.Visible = true;
            reminderNotification.BalloonTipTitle = title;
            reminderNotification.BalloonTipText = message;
            reminderNotification.ShowBalloonTip(3000);
        }
        #endregion

        #region Buttons
        private void btnSettings_Click(object sender, EventArgs e) {
            FormSettings settingsForm = new FormSettings();
            settingsForm.CityChanged += async (s, ev) => await GetPrayerTimes(); // Event'i dinliyoruz

            // Form1'in konumunu al
            int form1X = this.Location.X;
            int form1Y = this.Location.Y;

            // Ekran geniþliðini al
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;

            // FormSettings'in geniþliðini al
            int settingsWidth = settingsForm.Width;

            // FormSettings'in konumunu ayarla
            settingsForm.StartPosition = FormStartPosition.Manual;

            if (form1X - settingsWidth - 2 >= 0) // Sol kenarda yeterince yer var mý?
            {
                settingsForm.Location = new Point(form1X - settingsWidth - 2, form1Y); // Sol üst kenara al
            } else {
                settingsForm.Location = new Point(form1X + this.Width + 2, form1Y); // Sað tarafa al
            }

            settingsForm.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e) => Application.Exit();
        #endregion

        #region Movable Form
        private Point mouseLocation;
        private void Form1_MouseMove(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) this.Location = new Point(Control.MousePosition.X + mouseLocation.X, Control.MousePosition.Y + mouseLocation.Y); }
        private void Form1_MouseDown(object sender, MouseEventArgs e) => mouseLocation = new Point(-e.X, -e.Y);
        #endregion

        #region Change Widget Size
        private void lblChangeSize_Click(object sender, EventArgs e) {
            if (this.Size == new System.Drawing.Size(220, 300)) this.Size = new System.Drawing.Size(220, 130);
            else if (this.Size == new System.Drawing.Size(220, 130)) this.Size = new System.Drawing.Size(220, 300);
        }
        #endregion

        #region Startup Position
        private void SetFormPosition() {
            int pointX = Screen.PrimaryScreen.Bounds.Width - this.Width - 50; // Sað kenardan 50 piksel içeri
            int pointY = 50; // Üst kenardan 50 piksel aþaðý
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(pointX, pointY);
        }
        #endregion

    }
}
