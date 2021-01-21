using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace WeatherReminders.Function
{
    class Weather
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<Tuple<string, bool>> GetJacketTypeAndUmbrellaNeeds()
        {
            var hourlyForcasts = await GetHourlyForcasts();
            List<double>[] hourlyTempsAndPercipProbs = GetHourlyTempsAndPercipProbs(hourlyForcasts);
            List<double> hourlyTemps = hourlyTempsAndPercipProbs[0];
            List<double> hourlyPercipProbs = hourlyTempsAndPercipProbs[1];
            string jacketType = GetJacketType(hourlyTemps);
            bool umbrellaNeed = GetUmbrellaNeed(hourlyPercipProbs);
            var jacketTypeAndUmbrellaNeeds = Tuple.Create<string, bool>(jacketType, umbrellaNeed);
            return jacketTypeAndUmbrellaNeeds;
        }

        private static async Task<List<HourlyForcast>> GetHourlyForcasts()
        {
            double lat = Convert.ToDouble(Environment.GetEnvironmentVariable("OPEN_WEATHER_MAP_LAT"));
            double lon = Convert.ToDouble(Environment.GetEnvironmentVariable("OPEN_WEATHER_MAP_LON"));
            string units = "metric";
            string exclude = "current,minutely,daily,alerts";
            string appid = Environment.GetEnvironmentVariable("OPEN_WEATHER_MAP_APP_ID");
            string url = $"https://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&units={units}&exclude={exclude}&appid={appid}";
            var streamTask = client.GetStreamAsync(url);
            var forcast = await JsonSerializer.DeserializeAsync<Forcast>(await streamTask);
            return forcast.HourlyForcasts.GetRange(0, 16);  // only get first 16 hours of forcasts
        }

        private static List<double>[] GetHourlyTempsAndPercipProbs(List<HourlyForcast> hourlyForcasts)
        {
            List<double> hourlyTemps = new List<double>();
            List<double> hourlyPercipProbs = new List<double>();
            foreach (var hourlyForcast in hourlyForcasts)
            {
                hourlyTemps.Add(hourlyForcast.Temp);
                hourlyPercipProbs.Add(hourlyForcast.PercipProb);
            }
            List<double>[] hourlyTempsAndPercipProbs = {hourlyTemps, hourlyPercipProbs};
            return hourlyTempsAndPercipProbs;
        }

        private static string GetJacketType(List<double> hourlyTemps)
        {
            string jacketType = "";
            double minTemp = hourlyTemps.Min();
            if (minTemp <= 0)
            {
                jacketType = "thick jacket";
            }
            else if (minTemp <= 15)
            {
                jacketType = "moderately thick jacket";
            }
            else if (minTemp <= 22)
            {
                jacketType = "light jacket";
            }
            return jacketType;
        }

        private static bool GetUmbrellaNeed(List<double> hourlyPercipProbs)
        {
            foreach (double hourlyPercipProb in hourlyPercipProbs)
            {
                if (hourlyPercipProb > 0.5)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
