using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ramazan_2025 {
    class PrayerTimes {
        private static readonly Lazy<HttpClient> client = new Lazy<HttpClient>(() => {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.ConnectionClose = true; // Gereksiz bağlantıları kapat
            return httpClient;
        });

        private static HttpClient Client => client.Value;

        public async Task<(string Fajr, string Dhuhr, string Asr, string Maghrib, string Isha, string TomorrowFajr, string Day)> GetPrayerTimes(string selectedCity) {
            string todayDate = DateTime.Now.ToString("dd-MM-yyyy");
            string tomorrowDate = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");

            string todayUrl = $"http://api.aladhan.com/v1/timingsByCity/{todayDate}?city={selectedCity}&country=Turkey&method=13";
            string tomorrowUrl = $"http://api.aladhan.com/v1/timingsByCity/{tomorrowDate}?city={selectedCity}&country=Turkey&method=13";

            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true // Büyük-küçük harf duyarlılığını kaldır
            };

            using (var todayResponse = await Client.GetStreamAsync(todayUrl))
            using (var tomorrowResponse = await Client.GetStreamAsync(tomorrowUrl)) {
                var todayData = await JsonSerializer.DeserializeAsync<PrayerApiResponse>(todayResponse, options);
                var tomorrowData = await JsonSerializer.DeserializeAsync<PrayerApiResponse>(tomorrowResponse, options);

                if (todayData?.Data == null || tomorrowData?.Data == null)
                    throw new Exception("API'den gelen veri hatalı veya eksik!");

                return (
                    todayData.Data.Timings.Fajr,
                    todayData.Data.Timings.Dhuhr,
                    todayData.Data.Timings.Asr,
                    todayData.Data.Timings.Maghrib,
                    todayData.Data.Timings.Isha,
                    tomorrowData.Data.Timings.Fajr,
                    todayData.Data.Date.Hijri.Day
                );
            }
        }
    }

    public class PrayerApiResponse {
        public DataContainer Data { get; set; }
    }

    public class DataContainer {
        public Timings Timings { get; set; }
        public DateInfo Date { get; set; }
    }

    public class Timings {
        public string Fajr { get; set; }
        public string Dhuhr { get; set; }
        public string Asr { get; set; }
        public string Maghrib { get; set; }
        public string Isha { get; set; }
    }

    public class DateInfo {
        public HijriDate Hijri { get; set; }
    }

    public class HijriDate {
        public string Day { get; set; }
    }
}
