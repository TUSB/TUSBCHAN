using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TUSBCHAN.Functions;
using TUSBCHAN.Modules.EveryoneUser;

namespace TUSBCHAN
{
    class Program
    {
        public static DiscordSocketClient client;
        public static CommandService commands;
        public static IServiceProvider services;
        
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();


        public async Task MainAsync()
        {
            Common.IniFileLoad();

            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();
            client.MessageReceived += CommandRecieved;
            client.UserJoined += UserJoined;
            client.Disconnected += Disconnected;
            client.UserLeft += UserLeft;
            client.Log += Log;
            client.Ready += Start;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
            await client.LoginAsync(TokenType.Bot, Common.DiscordToken);
            await client.StartAsync();

            var twitter = new API.Twitter.Streaming(client);
            twitter.StartStreaming();

            await Task.Delay(-1);
        }

        private Task Start()
        {
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

            // 自動応答
            int argPos = 0;
            if (!message.HasCharPrefix('!',ref argPos))
            {
                await new Education().Reply(message);
            }

            // コマンド実行
            argPos = 0;
            // コマンドかどうか判定
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
            var sql = string.Format("SELECT * FROM ログイン WHERE サーバーID = '{0}'",
                user.Guild.Id);

            var table = DBAccesser.RunSQLGetResult(sql);
            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0];
                var channelId = ulong.Parse(row["チャンネルID"].ToString());
                var word = row["内容"].ToString();
                word = word.Replace("{ServerName}", user.Guild.Name);
                word = word.Replace("{UserName}", user.Username);
                await user.Guild.GetTextChannel(channelId).SendMessageAsync(word);
            }
        }

        /// <summary>
        /// ユーザーが会議から抜けたときの処理
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task UserLeft(SocketGuildUser user)
        {
            var sql = string.Format("SELECT * FROM ログアウト WHERE サーバーID = '{0}'",
                user.Guild.Id);

            var table = DBAccesser.RunSQLGetResult(sql);
            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0];
                var channelId = ulong.Parse(row["チャンネルID"].ToString());
                var word = row["内容"].ToString();
                word = word.Replace("{ServerName}", user.Guild.Name);
                word = word.Replace("{UserName}", user.Username);
                await user.Guild.GetTextChannel(channelId).SendMessageAsync(word);
            }
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
