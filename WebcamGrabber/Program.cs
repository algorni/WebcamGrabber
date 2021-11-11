using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebcamGrabber
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome WebCam Grabber!");

            string storageFolder = Environment.GetEnvironmentVariable("storageFolder");

            if (!string.IsNullOrEmpty(storageFolder))
            {
                Console.WriteLine($"Images will be stored in the following folder: {storageFolder}");
            }
            else
            {
                Console.WriteLine("Missing storageFolder env variable.");
                return;
            }

            string webcamUrl = Environment.GetEnvironmentVariable("webcamUrl");

            if (!string.IsNullOrEmpty(webcamUrl))
            {
                Console.WriteLine($"WebCam Url: {webcamUrl}");
            }
            else
            {
                Console.WriteLine("Missing webcamUrl env variable.");
                return;
            }


            string intervalStr = Environment.GetEnvironmentVariable("interval");

            if (!string.IsNullOrEmpty(intervalStr))
            {
                Console.WriteLine($"Interval: {intervalStr}");
            }
            else
            {
                Console.WriteLine("Missing interval env variable.");
                return;
            }

            int interval;
            bool intervalParsable = int.TryParse(intervalStr, out interval);

            if ( !intervalParsable)
            {
                Console.WriteLine("Interval is not an integer.");
                return;
            }

            HttpClient httpClient = new HttpClient();
            
            while (true)
            {
                byte[] image = await httpClient.GetByteArrayAsync(webcamUrl);

                DateTime now = DateTime.UtcNow;

                System.IO.File.WriteAllBytes($"{now.Year}-{now.Month}-{now.Day}-{now.Hour}-{now.Minute}-{now.Second}", image);

                await Task.Delay(TimeSpan.FromSeconds(interval));
            }
        }
    }
}
