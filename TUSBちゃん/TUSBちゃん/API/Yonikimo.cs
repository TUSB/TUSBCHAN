using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TUSBちゃん.API
{
    class Yonikimo
    {
        public EmbedBuilder GetYonikimo(int No)
        {
            string urlstring = $"http://yonikimo.com/{No}.html";

            var doc = new HtmlAgilityPack.HtmlDocument();

            // 指定したサイトのHTMLをストリームで取得する
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            string str = client.DownloadString(urlstring);
            doc.LoadHtml(str);

            // HTMLから(複数の)titleタグを探す
            var datas = doc.DocumentNode.Descendants("td");
            var child = datas.First().ChildNodes.First();
            var arasuji = child.InnerHtml.Replace("<br>", Environment.NewLine);
            var titles = doc.DocumentNode.Descendants("h3");
            var title = titles.First().InnerText;

            var eb = new EmbedBuilder()
            {
                Color = new Color(255, 0, 0),
                Title = title,
                Url = urlstring,
                Description = arasuji
            };

            return eb;
        }

        public int SearchYonikimoNo(string word)
        {
            string urlstring = "http://yonikimo.com/story.html";

            var doc = new HtmlAgilityPack.HtmlDocument();

            // 指定したサイトのHTMLをストリームで取得する
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            string str = client.DownloadString(urlstring);
            doc.LoadHtml(str);

            // HTMLから(複数の)titleタグを探す
            var datas = doc.DocumentNode.Descendants("td");
            bool flag = false;
            int no = 0;
            int tmp = 0;
            foreach (var data in datas)
            {
                if (int.TryParse(data.InnerText, out tmp))
                {
                    no = int.Parse(data.InnerText);
                    flag = true;
                }
                else
                {
                    if (flag)
                    {
                        var title = data.InnerText;
                        if (title.IndexOf(word) != -1)
                        {
                            return no;
                        }
                        flag = false;
                    }
                }
            }
            return -1;
        }
    }
}
