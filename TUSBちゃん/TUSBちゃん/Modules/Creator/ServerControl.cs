using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TUSBちゃん.Modules.Creator
{
    public class ServerControl : ModuleBase
    {
        private CommandService _service;
        private Functions.Decision decision = new Functions.Decision();
        private static API.MinecraftServer server = new API.MinecraftServer();

        public ServerControl(CommandService service)
        {
            _service = service;
        }

        [Command("start")]
        [Summary("製作サーバーを稼働させます *要Creator")]
        public async Task Start()
        {
            var task = Task.Run(() => server.StartServer());
            await ReplyAsync("サーバーを開始しました");
        }

        [Command("stop")]
        [Summary("製作サーバーを停止させます *要Creator")]
        public async Task Stop()
        {
            server.StopServer();
            await ReplyAsync("サーバーを停止しました");
        }

        [Command("tweet")]
        [Summary("TUSB公式アカウントでツイートをします *要Creator")]
        public async Task Tweet([Summary("呟く内容")] string tweet = null)
        {
            if (decision.IsCreator((SocketMessage)Context.Message))
            {
                if (tweet == null)
                {
                    await ReplyAsync("tweetコマンドの後に呟きたい内容を続けて入力してください\n" +
                        "例:「!tweet こんにちは」");
                }
                else
                {
                    var tservice = new API.Twitter.TwitterService();
                    tservice.Tweet(tweet);
                    await ReplyAsync("Twitterに呟きました");
                }
            }
            else
            {
                await ReplyAsync("実行権限がありません");
            }
        }

        [Command("reboot")]
        [Summary("TUSBちゃんを再起動します *要Creator")]
        public async Task Reboot()
        {
            if (decision.IsCreator((SocketMessage)Context.Message))
            {
                await ReplyAsync("TUSBちゃんを再起動します...");
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                Environment.Exit(0);
            }
            else
            {
                await ReplyAsync("実行権限がありません");
            }
        }
    }
}
