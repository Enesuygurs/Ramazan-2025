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
        private string TomorrowFajr; // Yar�n�n sahur vakti i�in de�i�ken
        private DateTime sonKontrolEdilenGun = DateTime.Now.Date; // Ba�lang��ta bug�n� sakl�yoruz
        private System.Windows.Forms.Label aktifLabel = null;

        private async Task GetNamazVakitleri() {
            string selectedCity = Properties.Settings.Default.SelectedCity;

            // �ehir se�ilmemi�se varsay�lan olarak �stanbul kullan
            if (string.IsNullOrEmpty(selectedCity)) {
                selectedCity = "�stanbul";
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

                // Bug�n�n namaz vakitlerini al
                Fajr = todayData.data.timings.Fajr;
                Dhuhr = todayData.data.timings.Dhuhr;
                Asr = todayData.data.timings.Asr;
                Maghrib = todayData.data.timings.Maghrib;
                Isha = todayData.data.timings.Isha;

                // Yar�n�n sahur vakti
                TomorrowFajr = tomorrowData.data.timings.Fajr;
                // Vakitleri aray�ze yazd�r
                lblTime1.Text = $"�msak: {Fajr}";
                lblTime2.Text = $"��le: {Dhuhr}";
                lblTime3.Text = $"�kindi: {Asr}";
                lblTime4.Text = $"Ak�am: {Maghrib}";
                lblTime5.Text = $"Yats�: {Isha}";
            }
            GC.Collect();
        }

        private async void timerKalanSure_Tick(object sender, EventArgs e) {
            DateTime simdikiZaman = DateTime.Now;
            DateTime today = simdikiZaman.Date;

            // E�er g�n de�i�tiyse, namaz vakitlerini g�ncelle
            if (sonKontrolEdilenGun != today) {
                await GetNamazVakitleri(); // Yeni namaz vakitlerini �ek
                sonKontrolEdilenGun = today; // Son kontrol edilen g�n� g�ncelle
            }

            DateTime sahurVakti = DateTime.Parse(Fajr);
            DateTime iftarVakti = DateTime.Parse(Maghrib);
            DateTime yarinSahurVakti = DateTime.Parse(TomorrowFajr);

            // Bug�ne eklenmi� saatleri olu�tur
            sahurVakti = today.Add(sahurVakti.TimeOfDay);
            iftarVakti = today.Add(iftarVakti.TimeOfDay);
            yarinSahurVakti = today.AddDays(1).Add(yarinSahurVakti.TimeOfDay);

            System.Windows.Forms.Label yeniAktifLabel = null;

            if (simdikiZaman < sahurVakti) {
                TimeSpan kalanSure = sahurVakti - simdikiZaman;
                lblKalanZaman.Text = $"Kalan S�re:\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                yeniAktifLabel = lblTime1;
                // **Bildirim Ayar� Aktifse ve 15 dakika kald�ysa bildirim g�ster
                if (Properties.Settings.Default.reminder && kalanSure.Hours == 0 && kalanSure.Minutes == 15 && kalanSure.Seconds == 0) ShowNotification("Sahur Vakti Yakla��yor!", "Sahura 15 dakika kald�.");
            } else if (simdikiZaman > sahurVakti && simdikiZaman < iftarVakti) {
                TimeSpan kalanSure = iftarVakti - simdikiZaman;
                lblKalanZaman.Text = $"Kalan S�re:\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                yeniAktifLabel = lblTime4;
                // **Bildirim Ayar� Aktifse ve 15 dakika kald�ysa bildirim g�ster
                if (Properties.Settings.Default.reminder && kalanSure.Hours == 0 && kalanSure.Minutes == 15 && kalanSure.Seconds == 0) ShowNotification("�ftar Vakti Yakla��yor!", "�ftara 15 dakika kald�.");
            } else {
                TimeSpan kalanSure = yarinSahurVakti - simdikiZaman;
                lblKalanZaman.Text = $"Kalan S�re:\n{kalanSure.Hours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
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
