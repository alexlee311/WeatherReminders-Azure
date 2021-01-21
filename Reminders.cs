using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace WeatherReminders.Function {
    class Reminders
    {
        public static string GenerateMessageText(string jacketType, bool umbrellaNeed)
        {
            string messageText = "";
            if (jacketType != "")
            {
                messageText = $"Weather Reminders:\nWear a {jacketType}.";
            }
            if (umbrellaNeed)
            {
                messageText += "\nBring an umbrella.";
            }
            return messageText;
        }
        public static void SendReminder(string messageText)
        {
            string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            string fromNumber = Environment.GetEnvironmentVariable("TWILIO_FROM_NUMBER");
            string toNumber = Environment.GetEnvironmentVariable("TWILIO_TO_NUMBER");
            TwilioClient.Init(accountSid, authToken);
            var message = MessageResource.Create(
                body: messageText,
                from: new Twilio.Types.PhoneNumber(fromNumber),
                to: new Twilio.Types.PhoneNumber(toNumber)
            );
        }
    }   
}