using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebcamToLobe
{
    public class iftttHelper
    {
        private static HttpClient httpClient = new HttpClient();

        private static string iftttEndpoint = null;

        

        private static void ensureiftttEndpoint()
        {
            iftttEndpoint = Environment.GetEnvironmentVariable("iftttEndpoint");

            if (!string.IsNullOrEmpty(iftttEndpoint))
            {
                Console.WriteLine($"ifttt Endpoint: {iftttEndpoint}");
            }
            else
            {
                Console.WriteLine("Missing iftttEndpoint env variable.");
                return;
            }
        }

        public static void sendNotification(statusObjects statusObjects)
        {
            ensureiftttEndpoint();

            string notificationPayload = JsonSerializer.Serialize(statusObjects);

            var content = new StringContent(notificationPayload, Encoding.UTF8, "application/json");

            var httpResponse = httpClient.PostAsync(iftttEndpoint, content).Result;
        }

    }
}
