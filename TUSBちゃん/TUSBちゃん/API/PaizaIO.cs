using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace TUSBちゃん.API
{
    class PaizaIO
    {
        public string GetTitle(string id)
        {
            string _url = $"https://paiza.io/api/projects/{id}.json";
            string title = "";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                string text = wc.DownloadString(_url);
                wc.Dispose();
                var res = (JObject)JsonConvert.DeserializeObject(text);

                title = (string)res.GetValue("title");
            }

            return title;
        }

        public string GetCodeOut(string id)
        {
            string _url = $"https://paiza.io/api/projects/{id}.json";
            string title = "";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                string text = wc.DownloadString(_url);
                wc.Dispose();
                var res = (JObject)JsonConvert.DeserializeObject(text);

                title = (string)res.GetValue("stdout");
            }

            return title;
        }
    }
}
