using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebcamToLobe
{
    public class statusObjectsHelper
    {
        private static HttpClient httpClient = new HttpClient();

        private static string apiKey = null;

        private static string jsonUrl = null; 

        private static void ensureApiKey()
        {
            apiKey = Environment.GetEnvironmentVariable("statusObjectsApiKey");
            jsonUrl = Environment.GetEnvironmentVariable("statusObjectsUrl");

            if (!httpClient.DefaultRequestHeaders.Contains("Api-Key"))
            {
                httpClient.DefaultRequestHeaders.Add("Api-Key", apiKey);
            }
        }

        public static statusObjects getStatus()
        {
            ensureApiKey();

            var httpResponse = httpClient.GetAsync(jsonUrl).Result;

            statusObjects statusObjectsResponse = JsonSerializer.Deserialize<statusObjects>(httpResponse.Content.ReadAsStringAsync().Result);
            
            return statusObjectsResponse;
        }

        public static void putStatus(statusObjects statusObjects)
        {
            ensureApiKey();

            string statusObjectsString = JsonSerializer.Serialize(statusObjects);

            var content = new StringContent(statusObjectsString, Encoding.UTF8, "application/json");

            var httpResponse = httpClient.PutAsync(jsonUrl, content).Result;
        }

    }
}
