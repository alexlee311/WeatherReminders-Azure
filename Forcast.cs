using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WeatherReminders.Function
{
    class Forcast
    {
        [JsonPropertyName("hourly")]
        public List<HourlyForcast> HourlyForcasts { get; set; }
    }

    public class HourlyForcast
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("pop")]
        public double PercipProb { get; set; }
    }
}
