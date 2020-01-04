using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SantaTalk.Models;
using Xamarin.Essentials;

namespace SantaTalk
{
    public class LetterDeliveryService
    {
        //string santaUrl = "{REPLACE WITH YOUR FUNCTION URL}/api/WriteSanta";

        string santaUrl = "https://xsantawrite.azurewebsites.net/api/HttpTrigger1";
        //string santaUrl = "http://localhost:7071/api/WriteSanta";
        static HttpClient httpClient = new HttpClient();

        public async Task<SantaResults> WriteLetterToSanta(SantaLetter letter)
        {
            // if we're on the Android emulator, running functions locally, need to swap out the function url
            if (santaUrl.Contains("localhost") && DeviceInfo.DeviceType == DeviceType.Virtual && DeviceInfo.Platform == DevicePlatform.Android)
            {
                santaUrl = "http://10.0.2.2:7071/api/WriteSanta";
            }

            SantaResults results = null;
            try
            {
                letter.CountryHint = GetCountryByIP();
                var letterJson = JsonConvert.SerializeObject(letter);
                santaUrl = "https://xsantawrite.azurewebsites.net/api/HttpTrigger1?letterJson=" + letterJson;
                var httpResponse = await httpClient.PostAsync(santaUrl, null);
                results = JsonConvert.DeserializeObject<SantaResults>(await httpResponse.Content.ReadAsStringAsync());
                return results;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                results = new SantaResults { SentimentScore = -1 };
            }

            return results;
        }
        public static string GetCountryByIP()
        {
            IpInfo ipInfo = new IpInfo();

            string info = new WebClient().DownloadString("http://ipinfo.io");

            ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);

            return ipInfo.country;
           
        }
        public class IpInfo
        {
            public string ip { get; set; }
            public string city { get; set; }
            public string region { get; set; }
            public string country { get; set; }
            public string loc { get; set; }
            public string org { get; set; }
            public string postal { get; set; }
            public string timezone { get; set; }
            public string readme { get; set; }
        }
    }
}
