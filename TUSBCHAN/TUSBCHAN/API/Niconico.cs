using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace TUSBCHAN.API
{
    class Niconico
    {
        public string SearchingIllust(string SearchWord)
        {
            string _url = $"http://api.search.nicovideo.jp/api/v2/illust/contents/search?q={SearchWord}&targets=tags&fields=contentId,title,description,tags,categoryTags,viewCounter,mylistCounter,commentCounter,startTime,thumbnailUrl&_sort=viewCounter&_context=apiguide";
            

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(_url);
            wr.ContentType = "application/json;charset=UTF-8";
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();
            string responseStr = null;

      
            string formitem = "{\"data\"{\"startTime\"}";
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
            catch (Exception)
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
            var res = (JObject)JsonConvert.DeserializeObject(responseStr);
            SearchWord = (string)((JValue)res.GetValue("context")).Value;
            return responseStr.ToString();
        }
        
    }
}
