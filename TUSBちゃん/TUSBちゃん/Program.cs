using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TUSBちゃん
{
    class Program
    {
        public static DiscordSocketClient client;
        public static CommandService commands;
        public static IServiceProvider services;
        private Library.IniFileWrapper ini = new Library.IniFileWrapper();

        private string officialsite = "https://skyblock.jp";
        
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();


        public async Task MainAsync()
        {
            
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();
            client.MessageReceived += CommandRecieved;
            client.UserJoined += UserJoined;
            client.Disconnected += Disconnected;
            client.UserLeft += UserLeft;
            client.Log += Log;
            client.Ready += Start;

            string token = ini["Discord", "Token"];
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            var twitter = new API.Twitter.Streaming(client);
            twitter.StartStreaming();

            await Task.Delay(-1);
        }

        private Task Start()
        {
            //CPU監視がうるさかったので止めた
            //var task = Task.Run(() => API.CPU.WatchingStart(client));
            return Task.CompletedTask;
        }

        /// <summary>
        /// 何かしらのメッセージの受信
        /// </summary>
        /// <param name="msgParam"></param>
        /// <returns></returns>
        private async Task CommandRecieved(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            Console.WriteLine("{0} {1}:{2}", message.Channel.Name, message.Author.Username, message);
            //メッセージがnullの場合
            if (message == null)
                return;
            if (message.Author.IsBot)
                return;

            

            if (message.Content.Substring(0,1) != "!")
            {
                bool isreply = await new Modules.EveryoneUser.Education().Reply(message);

                if (!isreply)
                {
                    //翻訳処理
                    if (new Functions.Decision().IsEnglish(messageParam))
                    {
                        var authtoken = new API.Translator.AzureAuthToken(ini["Azure", "Token"]);
                        var translate = new API.Translator.Translate();
                        var token = authtoken.GetAccessToken();
                        var text = translate.TranslateMethod(token, message.Content);
                        if (text != message.Content)
                        {
                            var channel = message.Channel as SocketTextChannel;
                            await channel.SendMessageAsync(text);
                        }
                    }
                }
            }

            await new Modules.EveryoneUser.Coding().Reply(message);

            int argPos = 0;

            //コマンドかどうか判定
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)))
                return;

            

            var context = new CommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, services);

            //実行できなかった場合
            if (!result.IsSuccess)
            {
                if (!(result.ErrorReason == "Unknown command."))
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
                
            }
        }

        /// <summary>
        /// ユーザーが会議に参加したときの処理
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task UserJoined(SocketGuildUser user)
        {
            //TUSBサーバー
            //Chatチャンネル
            var chatchannnel = client.GetChannel(290014442748379137) as SocketTextChannel;
            string welcome = string.Format("{0}様、ようこそTUSB会議へ！\nここではTUSBの最新情報や開発情報、意見やバグ報告などが行えます！\n\n" +
                "TUSB公式サイト {1}", user.Username, officialsite);
            await chatchannnel.SendMessageAsync(welcome);
        }

        /// <summary>
        /// ユーザーが会議から抜けたときの処理
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task UserLeft(SocketGuildUser user)
        {
            //Chatチャンネル
            var chatchannnel = client.GetChannel(290014442748379137) as SocketTextChannel;
            string bye = string.Format("{0}さんさようなら、またのお越しをお待ちしております", user.Username);
            await chatchannnel.SendMessageAsync(bye);
        }

        private Task Disconnected(Exception ex)
        {
            Console.WriteLine("異常を検知したためTUSBちゃんを再起動します...");
            System.Diagnostics.Process.Start(Application.ExecutablePath);
            Environment.Exit(0);
            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
