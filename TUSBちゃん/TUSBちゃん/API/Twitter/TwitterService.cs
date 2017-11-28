using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUSBちゃん.API.Twitter
{
    class TwitterService
    {
        private Tokens token;
        
        public TwitterService()
        {
            var ini = new Library.IniFileWrapper();
            token = CoreTweet.Tokens.Create(ini["Twitter","consumerKey"],
                ini["Twitter", "consumerSecret"],
                ini["Twitter", "accessToken"],
                ini["Twitter", "accessSecret"]);
        }
        
        public void Tweet(string tweet)
        {
            token.Statuses.Update(new { status = tweet });
        }
    }
}
