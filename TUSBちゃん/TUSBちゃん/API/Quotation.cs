﻿
using System.Xml;

namespace TUSBちゃん.API
{
    public class Quotation
    {
        public string GetQuot()
        {
            
            var _url = "http://meigen.doodlenote.net/api/";
            XmlDocument document = new XmlDocument();
            document.Load(_url);

            if (document.DocumentElement != null)
                foreach (XmlElement element in document.DocumentElement)
                {
                    string text = element.InnerText;                    
                    string meigen = document.GetElementsByTagName("meigen")[0].InnerText;
                    string auther = document.GetElementsByTagName("auther")[0].InnerText;

                    return $"`{meigen} \n 著: {auther}`";
                }
            return "取得失敗";
        }
    }
}