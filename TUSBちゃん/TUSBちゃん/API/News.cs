using System.Xml;

namespace TUSBちゃん.API
{
    class News
    {
        public string GetNews()
        {
            var _url = "https://news.yahoo.co.jp/pickup/rss.xml";
            XmlDocument document = new XmlDocument();
            document.Load(_url);
            var rss = document["rss"];
            var channel = rss["channel"];
            var item = channel["item"];

            var titile = item["title"].InnerText;
            var link = item["link"].InnerText;

            return $"{titile}\n{link}";
        }
    }
}
