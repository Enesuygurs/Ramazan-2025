using System.Runtime.InteropServices;

namespace Ramazan_2025 {
    public partial class Form1 : Form {

        #region Fields
        private string _fajr, _dhuhr, _asr, _maghrib, _isha;
        private string _tomorrowFajr;
        private DateTime _lastCheckedDay = DateTime.Now.Date;
        private PrayerTimes _prayerTimes = new PrayerTimes();
        private Label _activeLabel = null;
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

        private async void Form1_Load_1(object sender, EventArgs e) => await GetPrayerTimes();

        #region Ramadan Timetable Operations 
        public async Task GetPrayerTimes() {
            try {
                string selectedCity = Properties.Settings.Default.SelectedCity ?? "İstanbul";
                var result = await _prayerTimes.GetPrayerTimes(selectedCity);

                if (new[] { result.Fajr, result.Dhuhr, result.Asr, result.Maghrib, result.Isha, result.TomorrowFajr }.Any(string.IsNullOrEmpty)) {
                    MessageBox.Show("Prayer times are missing or incorrect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Başarıyla çekilen veriyi atama
                _fajr = result.Fajr;
                _dhuhr = result.Dhuhr;
                _asr = result.Asr;
                _maghrib = result.Maghrib;
                _isha = result.Isha;
                _tomorrowFajr = result.TomorrowFajr;

                // Arayüzü güncelleme
                lblTime1.Text = $"İmsak: {_fajr}";
                lblTime2.Text = $"Öğle: {_dhuhr}";
                lblTime3.Text = $"İkindi: {_asr}";
                lblTime4.Text = $"Akşam: {_maghrib}";
                lblTime5.Text = $"Yatsı: {_isha}";
                lblRamadanDay.Text = $"{result.Day}. Gün";

                if (!timerRemainingTime.Enabled) timerRemainingTime.Enabled = true;
            } catch (Exception ex) {
                MessageBox.Show($"An error occurred while fetching prayer times: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void timerremainingTime_Tick(object sender, EventArgs e) {
            try {
                DateTime currentTime = DateTime.Now;
                DateTime today = currentTime.Date;

                // Eğer gün değiştiyse, namaz vakitlerini güncelle
                if (_lastCheckedDay != today) {
                    await GetPrayerTimes(); // Yeni namaz vakitlerini çek
                    _lastCheckedDay = today; // Son kontrol edilen günü güncelle
                }

                DateTime suhoorTime, iftarVakti, tomorrowSuhoorTime;
                Label activeLabelNew = null;

                try {
                    // Null kontrolü ekleyerek hataları önlüyoruz
                    if (string.IsNullOrEmpty(_fajr) || string.IsNullOrEmpty(_maghrib) || string.IsNullOrEmpty(_tomorrowFajr)) {
                        MessageBox.Show("Namaz vakitleri yüklenemedi. Lütfen internet bağlantınızı kontrol edin.");
                    }

                    suhoorTime = DateTime.Parse(_fajr);
                    iftarVakti = DateTime.Parse(_maghrib);
                    tomorrowSuhoorTime = DateTime.Parse(_tomorrowFajr);

                    // Bugüne eklenmiş saatleri oluştur
                    suhoorTime = today.Add(suhoorTime.TimeOfDay);
                    iftarVakti = today.Add(iftarVakti.TimeOfDay);
                    tomorrowSuhoorTime = today.AddDays(1).Add(tomorrowSuhoorTime.TimeOfDay);
                } catch (Exception ex) {
                    MessageBox.Show($"An error occurred while fetching prayer times: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Hata olursa işlemi devam ettirme
                }

                if (currentTime < suhoorTime) {
                    TimeSpan remainingTime = suhoorTime - currentTime;
                    lblKalanZaman.Text = $"Kalan Süre\n{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                    activeLabelNew = lblTime1;
                    if (Properties.Settings.Default.reminder && remainingTime.Hours == 0 && remainingTime.Minutes == 15 && remainingTime.Seconds == 0) ShowNotification("Sahur Vakti Yaklaşıyor!", "Sahura 15 dakika kaldı.");
                } else if (currentTime > suhoorTime && currentTime < iftarVakti) {
                    TimeSpan remainingTime = iftarVakti - currentTime;
                    lblKalanZaman.Text = $"Kalan Süre\n{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                    activeLabelNew = lblTime4;
                    if (Properties.Settings.Default.reminder && remainingTime.Hours == 0 && remainingTime.Minutes == 15 && remainingTime.Seconds == 0) ShowNotification("İftar Vakti Yaklaşıyor!", "İftara 15 dakika kaldı.");
                } else {
                    TimeSpan remainingTime = tomorrowSuhoorTime - currentTime;
                    lblKalanZaman.Text = $"Kalan Süre\n{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                    activeLabelNew = lblTime1;
                }

                if (_activeLabel != activeLabelNew) {
                    if (_activeLabel != null) _activeLabel.ForeColor = Color.WhiteSmoke;
                    if (activeLabelNew != null) {
                        activeLabelNew.ForeColor = Color.Red;
                        _activeLabel = activeLabelNew;
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            int formX = this.Location.X;
            int formY = this.Location.Y;

            // Ekran genişliğini al
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;

            // FormSettings'in genişliğini al
            int settingsWidth = settingsForm.Width;

            // FormSettings'in konumunu ayarla
            settingsForm.StartPosition = FormStartPosition.Manual;

            if (formX - settingsWidth - 2 >= 0) // Sol kenarda yeterince yer var mı?
            {
                settingsForm.Location = new Point(formX - settingsWidth - 2, formY); // Sol üst kenara al
            } else {
                settingsForm.Location = new Point(formX + this.Width + 2, formY); // Sağ tarafa al
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
            int pointX = Screen.PrimaryScreen.Bounds.Width - this.Width - 50; // Sağ kenardan 50 piksel içeri
            int pointY = 50; // Üst kenardan 50 piksel aşağı
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(pointX, pointY);
        }
        #endregion
    }
}
