using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUSBCHAN.API.Docomo
{
    class ChatAI
    {
        private static Docomo.ChatResponse response = new ChatResponse();

        public static ChatResponse GetChat(string word,string name)
        {
            var dservice = new Docomo.DocomoService();
            response.utt = word;
            response = dservice.GetChat(response,name);

            return response;
        }
    }
}
