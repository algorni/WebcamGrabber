using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using lobe;
using lobe.ImageSharp;

namespace WebcamToLobe
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome WebCam to Lobe");
  



            #region get configuration

            var webcamUrl = Environment.GetEnvironmentVariable("webcamUrl");

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

            if (!intervalParsable)
            {
                Console.WriteLine("Interval is not an integer.");
                return;
            }


            var signatureFilePath = Environment.GetEnvironmentVariable("signatureFile");

            if (!string.IsNullOrEmpty(signatureFilePath))
            {
                Console.WriteLine($"Signature File: {signatureFilePath}");
            }
            else
            {
                Console.WriteLine("Missing Signature File (model) env variable.");
                return;
            }

            #endregion
                        
            
            ImageClassifier.Register("onnx", () => new OnnxImageClassifier());

            var classifier = OnnxImageClassifier.CreateFromSignatureFile(
                new FileInfo(signatureFilePath));                        

            HttpClient httpClient = new HttpClient();

            while (true)
            {
                byte[] imageBytes = await httpClient.GetByteArrayAsync(webcamUrl);
                
                var image = Image.Load(imageBytes).CloneAs<Rgb24>();

                //do the actual classification
                var results = classifier.Classify(image);

                var lastStatus = statusObjectsHelper.getStatus();

                if (results.Prediction.Label == "Nevicata")
                {
                    if (!lastStatus.data.isSnowing)
                    {
                        //do an action only if the last notification happened before the last 30 min if has value... if not..  just notify
                        if ( ((lastStatus.data.lastSnowingNotification.HasValue) & (DateTime.UtcNow  - lastStatus.data.lastSnowingNotification.Value > TimeSpan.FromMinutes(30)))                             
                            || (!lastStatus.data.lastSnowingNotification.HasValue))
                        {
                            lastStatus.data.lastSnowingNotification = DateTime.UtcNow;

                            lastStatus.data.lastSnowingStartTime = DateTime.UtcNow;
                            lastStatus.data.isSnowing = true;

                            iftttHelper.sendNotification(lastStatus);
                        }                        
                    }
                }
                else
                {
                    if (lastStatus.data.isSnowing)
                    {
                        //do an action only if the last notification happened before the last 30 min if has value... if not..  just notify
                        if (((lastStatus.data.lastSnowingNotification.HasValue) & (DateTime.UtcNow - lastStatus.data.lastSnowingNotification.Value > TimeSpan.FromMinutes(30)))
                            || (!lastStatus.data.lastSnowingNotification.HasValue))
                        {
                            lastStatus.data.lastSnowingNotification = DateTime.UtcNow;

                            lastStatus.data.lastSnowingEndTime = DateTime.UtcNow;
                            lastStatus.data.isSnowing = false;

                            iftttHelper.sendNotification(lastStatus);
                        }
                    }
                }
                
                await Task.Delay(TimeSpan.FromSeconds(interval));
            }
        }
    }
}
