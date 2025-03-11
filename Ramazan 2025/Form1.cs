using System.Runtime.InteropServices;

namespace Ramazan_2025
{
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
            await GetNamazVakitleri();
        }

        #region Ramadan Timetable Operations 
        public async Task GetNamazVakitleri() {
            string selectedCity = Properties.Settings.Default.SelectedCity ?? "Ýstanbul";

            // Namaz vakitlerini almak için NamazVakitleriManager sýnýfýný kullanýyoruz
            var (Fajr, Dhuhr, Asr, Maghrib, Isha, TomorrowFajr, Day) = await _prayerTimes.GetNamazVakitleri(selectedCity);

            // Alýnan namaz vakitlerini sýnýf içinde saklýyoruz
            this._fajr = Fajr;
            this._dhuhr = Dhuhr;
            this._asr = Asr;
            this._maghrib = Maghrib;
            this._isha = Isha;
            this._tomorrowFajr = TomorrowFajr;

            // Vakitleri arayüze yazdýrýyoruz
            lblTime1.Text = $"Ýmsak: {Fajr}";
            lblTime2.Text = $"Öðle: {Dhuhr}";
            lblTime3.Text = $"Ýkindi: {Asr}";
            lblTime4.Text = $"Akþam: {Maghrib}";
            lblTime5.Text = $"Yatsý: {Isha}";
            lblRamadanDay.Text = $"{Day}. Gün";
        }

        private async void timerKalanSure_Tick(object sender, EventArgs e) {
            DateTime simdikiZaman = DateTime.Now;
            DateTime today = simdikiZaman.Date;

            // Eðer gün deðiþtiyse, namaz vakitlerini güncelle
            if (_sonKontrolEdilenGun != today) {
                await GetNamazVakitleri(); // Yeni namaz vakitlerini çek
                _sonKontrolEdilenGun = today; // Son kontrol edilen günü güncelle
            }

            DateTime sahurVakti = DateTime.Parse(_fajr);
            DateTime iftarVakti = DateTime.Parse(_maghrib);
            DateTime yarinSahurVakti = DateTime.Parse(_tomorrowFajr);

            // Bugüne eklenmiþ saatleri oluþtur
            sahurVakti = today.Add(sahurVakti.TimeOfDay);
            iftarVakti = today.Add(iftarVakti.TimeOfDay);
            yarinSahurVakti = today.AddDays(1).Add(yarinSahurVakti.TimeOfDay);
            System.Windows.Forms.Label yeniAktifLabel = null;

            if (simdikiZaman < sahurVakti) {
                TimeSpan kalanSure = sahurVakti - simdikiZaman;
                lblKalanZaman.Text = $"Kalan Süre\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                yeniAktifLabel = lblTime1;
                // **Bildirim Ayarý Aktifse ve 15 dakika kaldýysa bildirim göster
                if (Properties.Settings.Default.reminder && kalanSure.Hours == 0 && kalanSure.Minutes == 15 && kalanSure.Seconds == 0) ShowNotification("Sahur Vakti Yaklaþýyor!", "Sahura 15 dakika kaldý.");
            } else if (simdikiZaman > sahurVakti && simdikiZaman < iftarVakti) {
                TimeSpan kalanSure = iftarVakti - simdikiZaman;
                lblKalanZaman.Text = $"Kalan Süre\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                yeniAktifLabel = lblTime4;
                // **Bildirim Ayarý Aktifse ve 15 dakika kaldýysa bildirim göster
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
            settingsForm.CityChanged += async (s, ev) => await GetNamazVakitleri(); // Event'i dinliyoruz

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
            if (this.Size == new System.Drawing.Size(220, 300))  this.Size = new System.Drawing.Size(220, 130);
            else if (this.Size == new System.Drawing.Size(220, 130))  this.Size = new System.Drawing.Size(220, 300);
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
