using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace TUSBCHAN.API
{
    class Weather
    {
        public string GetWeatherText(string day)
        {
            string _url = "http://weather.livedoor.com/forecast/webservice/json/v1?city=130010";
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            string text = wc.DownloadString(_url);
            wc.Dispose();
            var res = (JObject)JsonConvert.DeserializeObject(text);

            var forecasts = (JArray)res["forecasts"];
            for (int i = 0;i < forecasts.Count;i++)
            {
                var forecast = (JObject)forecasts[i];
                var dateLabel = (JValue)forecast["dateLabel"];
                var telop = (JValue)forecast["telop"];
                
                if ((string)dateLabel.Value == day)
                {
                    var location = (JObject)res["location"];
                    var city = (JValue)location["city"];

                    var description = (JObject)res["description"];
                    var weathertext = (JValue)description["text"];

                    return string.Format("{0}の{1}の天気は「{2}」でしょう。\n\n{3}", dateLabel, city, telop, weathertext);
                }
            }

            return "その日の天候情報を取得できませんでした。";
        }
    }
}