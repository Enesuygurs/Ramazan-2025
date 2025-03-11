using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ramazan_2025
{
    class PrayerTimes
    {
        public async Task<(string Fajr, string Dhuhr, string Asr, string Maghrib, string Isha, string TomorrowFajr, string Day)> GetNamazVakitleri(string selectedCity) {
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
                string Fajr = todayData.data.timings.Fajr;
                string Dhuhr = todayData.data.timings.Dhuhr;
                string Asr = todayData.data.timings.Asr;
                string Maghrib = todayData.data.timings.Maghrib;
                string Isha = todayData.data.timings.Isha;
                string Day = todayData.data.date.hijri.day;

                // Yarının sahur vakti
                string TomorrowFajr = tomorrowData.data.timings.Fajr;
                return (Fajr, Dhuhr, Asr, Maghrib, Isha, TomorrowFajr, Day);
            }
        }
    }
}
