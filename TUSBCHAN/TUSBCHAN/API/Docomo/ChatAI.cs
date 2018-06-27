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
        public static string GetChat(string word, string name)
        {
            var dservice = new Docomo.DocomoService();
            return dservice.GetChat(word, name);
        }
    }
}
