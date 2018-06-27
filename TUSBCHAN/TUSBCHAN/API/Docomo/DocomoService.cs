using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Web;

namespace TUSBCHAN.API.Docomo
{
    class DocomoService
    {
        private Library.IniFileAccesser ini = new Library.IniFileAccesser();


        public string GetChat(string word, string name)
        {
            string _url = "https://api.apigw.smt.docomo.ne.jp/naturalChatting/v1/dialogue?APIKEY=";
            _url += ini["Docomo", "APIKEY"];

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(_url);
            wr.ContentType = "application/json;charset=UTF-8";
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();
            string responseStr = null;

            string formitem = "{\"language\":\"ja-JP\",\"botId\":\"Chatting\",\"appId\":\"" + ini["Docomo", "ChatID"] + "\",\"voiceText\":\"" + word + "\",\"clientData\"";
            formitem += ":{\"option\":{\"place\":\"東京\",\"mode\":\"dialog\"}},\"appRecvTime\":\"" 
                + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"appSendTime\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"}";
            Console.WriteLine(formitem);
            byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
            rs.Write(formitembytes, 0, formitembytes.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                responseStr = reader2.ReadToEnd();
            }
            catch (Exception ex)
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            Console.WriteLine(responseStr);
            var res = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(responseStr);
            var systemText = (Newtonsoft.Json.Linq.JObject)res.GetValue("systemText");
            return (string)systemText.GetValue("expression");
        }

        public string GetQA(string word)
        {
            string urlEnc = HttpUtility.UrlEncode(word);
            string _url = "https://api.apigw.smt.docomo.ne.jp/knowledgeQA/v1/ask?APIKEY="+ ini["Docomo","APIKEY"] + "&q=" + word;
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            string text = wc.DownloadString(_url);

            var res = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(text);
            var message = (Newtonsoft.Json.Linq.JObject)res.GetValue("message");


            string textForDisplay = (string)message.GetValue("textForDisplay");
            Console.WriteLine(textForDisplay);

            try
            {
                var answers = (Newtonsoft.Json.Linq.JArray)res.GetValue("answers");
                textForDisplay += "\n";
                textForDisplay += "\n";
                textForDisplay += (string)((Newtonsoft.Json.Linq.JObject)answers[0]).GetValue("linkText") + "：";
                textForDisplay += (string)((Newtonsoft.Json.Linq.JObject)answers[0]).GetValue("linkUrl");
            }
            catch
            {
            }

            return textForDisplay;
        }

        public void Registration(string APIType)
        {
            string url = "";
            switch(APIType)
            {
                case "TaskRecog":
                    break;
                case "FAQ_Mobile":
                    break;
                case "Chatting":
                    url = "https://api.apigw.smt.docomo.ne.jp/naturalChatting/v1/registration";
                    break;
                case "Knowledge":
                    break;
            }

            url += "?APIKEY=";
            url += ini["Docomo", "APIKEY"];

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "application/json;charset=UTF-8";
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();
            string responseStr = null;

            string formitem = "{\"botId\": \"Chatting\",\"appKind\": \"DiscordBot\"}";
            Console.WriteLine(formitem);
            byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
            rs.Write(formitembytes, 0, formitembytes.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                responseStr = reader2.ReadToEnd();
            }
            catch (Exception ex)
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

        }
    }
}
