using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ramazan_2025 {
    public class PrayerTimes {
        private static readonly HttpClient _httpClient = new() {
            DefaultRequestHeaders = { ConnectionClose = true }
        };

        private readonly JsonSerializerOptions _jsonOptions = new() {
            PropertyNameCaseInsensitive = true
        };

        public async Task<PrayerTimesDto> GetPrayerTimesAsync(string city) {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("Şehir adı boş olamaz.", nameof(city));

            string todayUrl = BuildApiUrl(city, DateTime.UtcNow);
            string tomorrowUrl = BuildApiUrl(city, DateTime.UtcNow.AddDays(1));

            PrayerApiResponse todayData = await FetchPrayerTimesAsync(todayUrl);
            PrayerApiResponse tomorrowData = await FetchPrayerTimesAsync(tomorrowUrl);

            if (todayData?.Data == null || tomorrowData?.Data == null)
                throw new InvalidOperationException("API'den gelen veri hatalı veya eksik!");

            return new PrayerTimesDto {
                Fajr = todayData.Data.Timings.Fajr,
                Dhuhr = todayData.Data.Timings.Dhuhr,
                Asr = todayData.Data.Timings.Asr,
                Maghrib = todayData.Data.Timings.Maghrib,
                Isha = todayData.Data.Timings.Isha,
                TomorrowFajr = tomorrowData.Data.Timings.Fajr,
                HijriDay = todayData.Data.Date.Hijri.Day
            };
        }

        private static string BuildApiUrl(string city, DateTime date) =>
            $"http://api.aladhan.com/v1/timingsByCity/{date:dd-MM-yyyy}?city={city}&country=Turkey&method=13";

        private async Task<PrayerApiResponse> FetchPrayerTimesAsync(string url) {
            using HttpResponseMessage response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<PrayerApiResponse>(stream, _jsonOptions);
        }
    }

    public class PrayerTimesDto {
        public string Fajr { get; set; }
        public string Dhuhr { get; set; }
        public string Asr { get; set; }
        public string Maghrib { get; set; }
        public string Isha { get; set; }
        public string TomorrowFajr { get; set; }
        public string HijriDay { get; set; }
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
