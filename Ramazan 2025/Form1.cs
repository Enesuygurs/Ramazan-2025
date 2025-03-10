using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Reflection.Emit;

namespace Ramazan_2025
{
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }

        private string Fajr, Dhuhr, Asr, Maghrib, Isha;
        private string TomorrowFajr; // Yarýnýn sahur vakti için deðiþken
        private DateTime sonKontrolEdilenGun = DateTime.Now.Date; // Baþlangýçta bugünü saklýyoruz
        private System.Windows.Forms.Label aktifLabel = null;

        private async Task GetNamazVakitleri() {
            string selectedCity = Properties.Settings.Default.SelectedCity;

            // Þehir seçilmemiþse varsayýlan olarak Ýstanbul kullan
            if (string.IsNullOrEmpty(selectedCity)) {
                selectedCity = "Ýstanbul";
            }
            using (HttpClient client = new HttpClient()) {
                string todayDate = DateTime.Now.ToString("dd-MM-yyyy");
                string tomorrowDate = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");

                string todayUrl = $"http://api.aladhan.com/v1/timingsByCity/{todayDate}?city={selectedCity}&country=Turkey&method=13";
                string tomorrowUrl = $"http://api.aladhan.com/v1/timingsByCity/{tomorrowDate}?city={selectedCity}&country=Turkey&method=13";

                var todayResponse = await client.GetStringAsync(todayUrl);
                var tomorrowResponse = await client.GetStringAsync(tomorrowUrl);

                dynamic todayData = JsonConvert.DeserializeObject(todayResponse);
                dynamic tomorrowData = JsonConvert.DeserializeObject(tomorrowResponse);

                // Bugünün namaz vakitlerini al
                Fajr = todayData.data.timings.Fajr;
                Dhuhr = todayData.data.timings.Dhuhr;
                Asr = todayData.data.timings.Asr;
                Maghrib = todayData.data.timings.Maghrib;
                Isha = todayData.data.timings.Isha;

                // Yarýnýn sahur vakti
                TomorrowFajr = tomorrowData.data.timings.Fajr;
                // Vakitleri arayüze yazdýr
                lblTime1.Text = $"Ýmsak: {Fajr}";
                lblTime2.Text = $"Öðle: {Dhuhr}";
                lblTime3.Text = $"Ýkindi: {Asr}";
                lblTime4.Text = $"Akþam: {Maghrib}";
                lblTime5.Text = $"Yatsý: {Isha}";
            }
            GC.Collect();
        }

        private async void timerKalanSure_Tick(object sender, EventArgs e) {
            DateTime simdikiZaman = DateTime.Now;
            DateTime today = simdikiZaman.Date;

            // Eðer gün deðiþtiyse, namaz vakitlerini güncelle
            if (sonKontrolEdilenGun != today) {
                await GetNamazVakitleri(); // Yeni namaz vakitlerini çek
                sonKontrolEdilenGun = today; // Son kontrol edilen günü güncelle
            }

            DateTime sahurVakti = DateTime.Parse(Fajr);
            DateTime iftarVakti = DateTime.Parse(Maghrib);
            DateTime yarinSahurVakti = DateTime.Parse(TomorrowFajr);

            // Bugüne eklenmiþ saatleri oluþtur
            sahurVakti = today.Add(sahurVakti.TimeOfDay);
            iftarVakti = today.Add(iftarVakti.TimeOfDay);
            yarinSahurVakti = today.AddDays(1).Add(yarinSahurVakti.TimeOfDay);

            System.Windows.Forms.Label yeniAktifLabel = null;

            if (simdikiZaman < sahurVakti) {
                TimeSpan kalanSure = sahurVakti - simdikiZaman;
                lblKalanZaman.Text = $"Kalan Süre:\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                yeniAktifLabel = lblTime1;
                // **Bildirim Ayarý Aktifse ve 15 dakika kaldýysa bildirim göster
                if (Properties.Settings.Default.reminder && kalanSure.Hours == 0 && kalanSure.Minutes == 15 && kalanSure.Seconds == 0) ShowNotification("Sahur Vakti Yaklaþýyor!", "Sahura 15 dakika kaldý.");
            } else if (simdikiZaman > sahurVakti && simdikiZaman < iftarVakti) {
                TimeSpan kalanSure = iftarVakti - simdikiZaman;
                lblKalanZaman.Text = $"Kalan Süre:\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                yeniAktifLabel = lblTime4;
                // **Bildirim Ayarý Aktifse ve 15 dakika kaldýysa bildirim göster
                if (Properties.Settings.Default.reminder && kalanSure.Hours == 0 && kalanSure.Minutes == 15 && kalanSure.Seconds == 0) ShowNotification("Ýftar Vakti Yaklaþýyor!", "Ýftara 15 dakika kaldý.");
            } else {
                TimeSpan kalanSure = yarinSahurVakti - simdikiZaman;
                lblKalanZaman.Text = $"Kalan Süre:\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                yeniAktifLabel = lblTime1;
            }

            if (aktifLabel != yeniAktifLabel) {
                if (aktifLabel != null) aktifLabel.ForeColor = Color.WhiteSmoke;
                if (yeniAktifLabel != null) {
                    yeniAktifLabel.ForeColor = Color.Red;
                    aktifLabel = yeniAktifLabel;
                }
            }
        }
        private void ShowNotification(string title, string message) {
            reminderNotification.Icon = SystemIcons.Information;
            reminderNotification.Visible = true;
            reminderNotification.BalloonTipTitle = title;
            reminderNotification.BalloonTipText = message;
            reminderNotification.ShowBalloonTip(3000);
        }

        private async void Form1_Load_1(object sender, EventArgs e) {
            //Properties.Settings.Default.Reset();
            //Properties.Settings.Default.Save(); 
            await GetNamazVakitleri();
        }

        private void btnSettings_Click(object sender, EventArgs e) {
            FormSettings settingsForm = new FormSettings();
            settingsForm.ShowDialog();
        }
        private void btnClose_Click(object sender, EventArgs e) => Application.Exit();

        #region Movable Form
        private Point mouseLocation;
        private void Form1_MouseMove(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) this.Location = new Point(Control.MousePosition.X + mouseLocation.X, Control.MousePosition.Y + mouseLocation.Y); }
        private void Form1_MouseDown(object sender, MouseEventArgs e) => mouseLocation = new Point(-e.X, -e.Y);
        #endregion
    }
}
