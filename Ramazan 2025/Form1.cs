using System.Runtime.InteropServices;

namespace Ramazan_2025 {
    public partial class Form1 : Form {

        #region Fields
        private string _fajr, _dhuhr, _asr, _maghrib, _isha;
        private string _tomorrowFajr;
        private DateTime _lastCheckedDay = DateTime.Now.Date;
        private PrayerTimes _prayerTimes = new PrayerTimes();
        private Label _activeLabel = null;
        private FormSettings _settingsForm;
        #endregion

        #region Components
        // Pencereyi en alta almak için
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
                lblCity.Text = selectedCity;

                var result = await _prayerTimes.GetPrayerTimesAsync(selectedCity);
                if (new[] { result.Fajr, result.Dhuhr, result.Asr, result.Maghrib, result.Isha, result.TomorrowFajr }.Any(string.IsNullOrEmpty)) {
                    MessageBox.Show("Prayer times are missing or incorrect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                (_fajr, _dhuhr, _asr, _maghrib, _isha, _tomorrowFajr) = (result.Fajr, result.Dhuhr, result.Asr, result.Maghrib, result.Isha, result.TomorrowFajr);

                // Arayüzü güncelleme
                lblTime1.Text = $"İmsak: {_fajr}";
                lblTime2.Text = $"Öğle: {_dhuhr}";
                lblTime3.Text = $"İkindi: {_asr}";
                lblTime4.Text = $"Akşam: {_maghrib}";
                lblTime5.Text = $"Yatsı: {_isha}";
                lblRamadanDay.Text = $"{result.HijriDay}. Gün";

                if (!timerRemainingTime.Enabled) timerRemainingTime.Enabled = true;
            } catch (Exception ex) {
                MessageBox.Show($"An error occurred while fetching prayer times: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void timerRemainingTime_Tick(object sender, EventArgs e) {
            try {
                DateTime currentTime = DateTime.Now;
                DateTime today = currentTime.Date;

                // Eğer gün değiştiyse, namaz vakitlerini güncelle
                if (_lastCheckedDay != today) {
                    await GetPrayerTimes();
                    _lastCheckedDay = today;
                }
                if (string.IsNullOrWhiteSpace(_fajr) || string.IsNullOrWhiteSpace(_maghrib) || string.IsNullOrWhiteSpace(_tomorrowFajr)) return;

                // Tarihleri oluştur
                DateTime suhoorTime = today.Add(DateTime.Parse(_fajr).TimeOfDay);
                DateTime iftarTime = today.Add(DateTime.Parse(_maghrib).TimeOfDay);
                DateTime tomorrowSuhoorTime = today.AddDays(1).Add(DateTime.Parse(_tomorrowFajr).TimeOfDay);

                // Hangi Label aktif olacak?
                Label activeLabelNew = null;
                TimeSpan remainingTime;

                if (currentTime < suhoorTime) {
                    remainingTime = suhoorTime - currentTime;
                    activeLabelNew = lblTime1;
                    CheckReminder(remainingTime, "Sahur Vakti Yaklaşıyor!", "Sahura 15 dakika kaldı.");
                } else if (currentTime < iftarTime) {
                    remainingTime = iftarTime - currentTime;
                    activeLabelNew = lblTime4;
                    CheckReminder(remainingTime, "İftar Vakti Yaklaşıyor!", "İftara 15 dakika kaldı.");
                } else {
                    remainingTime = tomorrowSuhoorTime - currentTime;
                    activeLabelNew = lblTime1;
                }

                lblKalanZaman.Text = $"Kalan Süre\n{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";

                // Label renk değişimi yönetimi
                if (_activeLabel != activeLabelNew) {
                    if (_activeLabel != null) {
                        _activeLabel.ForeColor = Color.WhiteSmoke;
                    }

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
        private void CheckReminder(TimeSpan remainingTime, string title, string message) {
            if (Properties.Settings.Default.reminder && remainingTime.Hours == 0 && remainingTime.Minutes == 15 && remainingTime.Seconds == 0) {
                ShowNotification(title, message);
            }
        }

        private void ShowNotification(string title, string message) {
            reminderNotification.Visible = true;
            reminderNotification.BalloonTipTitle = title;
            reminderNotification.BalloonTipText = message;
            reminderNotification.ShowBalloonTip(3000);
        }
        #endregion

        #region Buttons
        private void btnSettings_Click(object sender, EventArgs e) {
            if (_settingsForm == null || _settingsForm.IsDisposed) {
                _settingsForm = new FormSettings();
                _settingsForm.CityChanged += OnCityChanged; // Tek seferlik ekleniyor
            }

            // Formu konumlandır
            PositionSettingsForm();

            // Formu göster
            _settingsForm.ShowDialog();
        }


        private async void OnCityChanged(object sender, EventArgs e) {
            try {
                await GetPrayerTimes();
            } catch (Exception ex) {
                MessageBox.Show($"Error fetching prayer times: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PositionSettingsForm() {
            int formX = this.Location.X;
            int formY = this.Location.Y;
            int settingsWidth = _settingsForm.Width;

            _settingsForm.StartPosition = FormStartPosition.Manual;

            if (formX - settingsWidth - 2 >= 0) {
                _settingsForm.Location = new Point(formX - settingsWidth - 2, formY);
            } else {
                _settingsForm.Location = new Point(formX + this.Width + 2, formY);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Application.Exit();
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
        #endregion

        #region Movable Form
        private Point mouseLocation;
        private void Form1_MouseMove(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) this.Location = new Point(Control.MousePosition.X + mouseLocation.X, Control.MousePosition.Y + mouseLocation.Y); }
        private void Form1_MouseDown(object sender, MouseEventArgs e) => mouseLocation = new Point(-e.X, -e.Y);
        #endregion

        #region Change Widget Size
        private void lblChangeSize_Click(object sender, EventArgs e) {
            if (this.Size == new System.Drawing.Size(220, 310)) this.Size = new System.Drawing.Size(220, 140);
            else if (this.Size == new System.Drawing.Size(220, 140)) this.Size = new System.Drawing.Size(220, 310);
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
