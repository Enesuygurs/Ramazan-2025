using System.Diagnostics;
using System.Runtime.InteropServices;
using static System.Net.WebRequestMethods;

namespace Ramazan_2025 {
    public partial class Form1 : Form {

        #region Fields
        private DateTime _lastCheckedDay = DateTime.Now.Date;
        private PrayerTimes _prayerTimes = new PrayerTimes();
        private Label? _activeLabel = null;
        private FormSettings? _settingsForm;
        private string? _fajr, _dhuhr, _asr, _maghrib, _isha, _tomorrowFajr;
        private const int OriginalWidth = 220;
        private const int OriginalHeight = 310;
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
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private const int WM_NCHITTEST = 0x84;
        private const int RESIZE_BORDER = 8;
        #endregion

        public Form1() {
            InitializeComponent();
            SetWindowPos(this.Handle, (IntPtr)HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW); // Widget olarak ayarla
            SetFormPosition(); // Formun konumunu ayarlayan metod
        }

        private async void Form1_Load(object sender, EventArgs e) => await GetPrayerTimes();

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
                Label? activeLabelNew = null;
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


        private async void OnCityChanged(object? sender, EventArgs e) {
            try {
                await GetPrayerTimes();
            } catch (Exception ex) {
                MessageBox.Show($"Error fetching prayer times: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PositionSettingsForm() {
            int formX = this.Location.X;
            int formY = this.Location.Y;
            if (_settingsForm != null) {
                int settingsWidth = _settingsForm.Width; // Varsayılan bir genişlik
                _settingsForm.StartPosition = FormStartPosition.Manual;
                if (formX - settingsWidth - 2 >= 0) {
                    _settingsForm.Location = new Point(formX - settingsWidth - 2, formY);
                } else {
                    _settingsForm.Location = new Point(formX + this.Width + 2, formY);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Application.Exit();
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
        #endregion
        
        #region Movable Form
        private void Form1_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ReleaseCapture(); // Kontrolü bırak
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0); // Formu taşı
            }
        }
        #endregion

        #region Change Widget Size
        private void lblChangeSize_Click(object sender, EventArgs e) {
            if (Height >= OriginalHeight) {
                MaximumSize = new Size(220, 140);
                Size = new Size(220, 140);
                lblChangeSize.Text = "⏷";
            } else {
                MinimumSize = new Size(220, 310);
                MaximumSize = new Size(225, 320);
                Size = new Size(220, 310);
                lblChangeSize.Text = "⏶";
            }
        }
        #endregion

        #region Startup Position & Resizable Form
        private void SetFormPosition() {
            int pointX = (Screen.PrimaryScreen != null) ? Screen.PrimaryScreen.Bounds.Width - this.Width - 50 : 100;
            int pointY = 50; // Üst kenardan 50 piksel aşağı
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(pointX, pointY);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ReleaseCapture();
                SendMessage(Handle, WM_NCHITTEST, HTCAPTION, 0);
            }
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == WM_NCHITTEST) {
                Point cursor = PointToClient(Cursor.Position);
                int w = Width, h = Height;

                if (cursor.X < RESIZE_BORDER) m.Result = (IntPtr)(cursor.Y < RESIZE_BORDER ? 13 : (cursor.Y > h - RESIZE_BORDER ? 16 : 10)); 
                else if (cursor.X > w - RESIZE_BORDER) m.Result = (IntPtr)(cursor.Y < RESIZE_BORDER ? 14 : (cursor.Y > h - RESIZE_BORDER ? 17 : 11));
                else if (cursor.Y < RESIZE_BORDER) m.Result = (IntPtr)12;
                else if (cursor.Y > h - RESIZE_BORDER) m.Result = (IntPtr)15;
                else base.WndProc(ref m);
                return;
            }
            base.WndProc(ref m);
        }

        private void Form1_Resize(object sender, EventArgs e) {
            if (OriginalWidth == 0 || OriginalHeight == 0 || Height < 310) return; // Bölme hatasını önlemek için kontrol
            // Form büyüklüğüne göre ölçeklendirme faktörü hesapla
            float widthScale = (float)this.ClientSize.Width / OriginalWidth;
            float heightScale = (float)this.ClientSize.Height / OriginalHeight;
            float scaleFactor = Math.Min(widthScale, heightScale); // Oranı minimuma göre ayarla

            int baseFontSize = 12; // Orijinal font boyutu
            int maxFontSize = 14;  // Maksimum font boyutu

            // Label fontlarını güncelle
            lblTime1.Font = new Font("Segoe UI Semibold", Math.Min(baseFontSize * scaleFactor, maxFontSize), FontStyle.Bold);
            lblTime2.Font = new Font("Segoe UI Semibold", Math.Min(baseFontSize * scaleFactor, maxFontSize), FontStyle.Bold);
            lblTime3.Font = new Font("Segoe UI Semibold", Math.Min(baseFontSize * scaleFactor, maxFontSize), FontStyle.Bold);
            lblTime4.Font = new Font("Segoe UI Semibold", Math.Min(baseFontSize * scaleFactor, maxFontSize), FontStyle.Bold);
            lblTime5.Font = new Font("Segoe UI Semibold", Math.Min(baseFontSize * scaleFactor, maxFontSize), FontStyle.Bold);

            // Kalan zaman label'ı biraz daha büyük başlasın
            lblKalanZaman.Font = new Font("Segoe UI Semibold", Math.Min(baseFontSize * scaleFactor, maxFontSize), FontStyle.Bold);
        }
        #endregion
    }
}
