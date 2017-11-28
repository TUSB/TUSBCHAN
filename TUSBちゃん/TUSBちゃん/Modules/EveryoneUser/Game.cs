using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUSBちゃん.Modules.EveryoneUser
{
    public class Game : ModuleBase
    {
        [Command("coin")]
        [Summary("コイントスを行いおもてかうらを決めます。")]
        public async Task Coin([Summary("コイントス回数")]int count = 1)
        {
            var sb = new StringBuilder();
            var rnd = new System.Random();
            int omote = 0;
            if (count == 0)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (rnd.Next(0, 2) == 0)
                {
                    sb.Append("おもて ");
                    omote++;
                }
                else
                {
                    sb.Append("うら ");
                }

            }
            
            await ReplyAsync(string.Format("結果:{0}\n表:{1}回\n裏:{2}回", sb.ToString(), omote, count - omote));
        }
    }
}
