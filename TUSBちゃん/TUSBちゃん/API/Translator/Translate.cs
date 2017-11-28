using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Web;

namespace TUSBちゃん.API.Translator
{
    class Translate
    {
        public string TranslateMethod(string authToken, string translating)
        {
            string translated = string.Empty;
            string from = "en";
            string to = "ja";
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" +
                HttpUtility.UrlEncode(translating) + "&from=" + from + "&to=" + to;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    DataContractSerializer dcs = new DataContractSerializer(Type.GetType("System.String"));
                    translated = (string)dcs.ReadObject(stream);
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
            return translated;
        }
    }
}
