using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace WeatherReminders.Function
{
    public static class WeatherReminders
    {
        [FunctionName("WeatherReminders")]
        public static async Task RunAsync([TimerTrigger("0 0 8 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var jacketTypeAndUmbrellaNeeds = await Weather.GetJacketTypeAndUmbrellaNeeds();
            string jacketType = jacketTypeAndUmbrellaNeeds.Item1;
            bool umbrellaNeed = jacketTypeAndUmbrellaNeeds.Item2;
            string messageText = Reminders.GenerateMessageText(jacketType, umbrellaNeed);
            if (messageText != "")
            {
                Reminders.SendReminder(messageText);
            }
        }
    }
}
