using System;
using System.Xml;

namespace TUSBちゃん.API.Quiz
{
    class Quiz
    {
        public QuizList GetQuiz()
        {
            var list = new QuizList();
            var _url = "http://24th.jp/test/quiz/api_quiz.php";
            while (string.IsNullOrEmpty(list.answer4))
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(_url);
                    var rss = document["Result"];
                    var channel = rss["quiz"];
                    list.question = channel["quession"].InnerText;
                    list.answer1 = channel["ans1"].InnerText;
                    list.answer2 = channel["ans2"].InnerText;
                    list.answer3 = channel["ans3"].InnerText;
                    list.answer4 = channel["ans4"].InnerText;
                }
                catch(Exception)
                {
                    list = new QuizList();
                }
            }
            
            return list;
        }
    }
}
