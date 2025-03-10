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
        public class NamazVakitleri {
            public string Fajr { get; set; }
            public string Dhuhr { get; set; }
            public string Asr { get; set; }
            public string Maghrib { get; set; }
            public string Isha { get; set; }
        }
        private async void GetNamazVakitleri(string city) {
            // Aladhan API'si i�in URL'yi olu�turuyoruz
            string url = $"http://api.aladhan.com/v1/timingsByCity?city={city}&country=Turkey&method=13";

            using (HttpClient client = new HttpClient()) {
                var response = await client.GetStringAsync(url);
                dynamic data = JsonConvert.DeserializeObject(response);

                // API'den al�nan namaz vakitlerini i�liyoruz
                NamazVakitleri namazVakitleri = new NamazVakitleri {
                    Fajr = data.data.timings.Fajr,
                    Dhuhr = data.data.timings.Dhuhr,
                    Asr = data.data.timings.Asr,
                    Maghrib = data.data.timings.Maghrib,
                    Isha = data.data.timings.Isha
                };

                // Vakitleri aray�ze yazd�r�yoruz
                label1.Text = "�msak: " + namazVakitleri.Fajr;
                label2.Text = "��le: " + namazVakitleri.Dhuhr;
                label3.Text = "�kindi: " + namazVakitleri.Asr;
                label4.Text = "Ak�am: " + namazVakitleri.Maghrib;
                label5.Text = "Yats�: " + namazVakitleri.Isha;
            }
        }

        private void btnGetVakitler_Click(object sender, EventArgs e) {

        }

        private void Form1_Load(object sender, EventArgs e) {
        }

        private void Form1_Load_1(object sender, EventArgs e) {
            GetNamazVakitleri("Sakarya");

        }
    }

}
