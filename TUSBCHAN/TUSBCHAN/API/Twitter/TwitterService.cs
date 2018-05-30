using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSBCHAN.Functions;

namespace TUSBCHAN.API.Twitter
{
    class TwitterService
    {
        private Tokens token;
        
        public TwitterService()
        {
            token = CoreTweet.Tokens.Create(
                Common.TwitterConsumerKey,
                Common.TwitterConsumerSecret,
                Common.TwitterAccessToken,
                Common.TwitterAccessSecret);
        }
        
        public void Tweet(string tweet)
        {
            token.Statuses.Update(new { status = tweet });
        }
    }
}
