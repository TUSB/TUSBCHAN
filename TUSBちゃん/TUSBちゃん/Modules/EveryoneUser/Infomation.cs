using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using TUSBちゃん.API;
using TUSBちゃん.API.Docomo;

//using SilverNBTLibrary;

namespace TUSBちゃん.Modules.EveryoneUser
{
    public class Infomation : ModuleBase
    {
        private CommandService _service;
        private Library.IniFileWrapper ini = new Library.IniFileWrapper();

        public Infomation(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("使えるコマンドの一覧を表示します")]
        public async Task Help()
        {
            var sb = new StringBuilder();
            sb.Append("```");
            foreach (var command in _service.Commands)
            {
                sb.AppendFormat("{0}：{1}\n", command.Name, command.Summary);
            }
            sb.Append("```");
            await ReplyAsync(sb.ToString());

        }

        [Command("now")]
        [Summary("サーバーが存在するタイムゾーンの現在時刻を表示します")]
        public async Task Now()
        {
            await ReplyAsync(DateTime.Now.ToString("yyyy年MM月dd日 tthh時mm分ss秒fffミリ秒"));
        }


        [Command("echo")]
        [Summary("言葉を発言させる")]
        public async Task Say([Remainder, Summary("発言させる言葉")] string echo = null)
        {
            if (echo == null)
            {
                await ReplyAsync("発言コマンドの後に発現させたい言葉を続けて入力してください\n" +
                                 "例:「!say 私の名前はTUSBちゃんと申します」");
            }
            else
            {
                await ReplyAsync(echo);
            }
        }

        [Command("ping")]
        [Summary("指定したアドレスにPingを送信します")]
        public async Task Ping([Remainder, Summary("アドレス")] string ip = null)
        {
            if (ip == null)
            {
                await ReplyAsync("pingコマンドの後にアドレスを続けて入力してください\n" +
                                 "例:「!ping skyblock.jp」");
            }
            else
            {
                //Pingオブジェクトの作成
                Ping p =
                    new Ping();
                //"www.yahoo.com"にPingを送信する
                PingReply reply = p.Send(ip);


                //結果を取得
                if (reply.Status == IPStatus.Success)
                {
                    string rep = string.Format("{0} からの応答:bytes={1} time={2}ms",
                        reply.Address, reply.Buffer.Length,
                        reply.RoundtripTime);
                    await ReplyAsync(rep);
                }
                else
                {
                    await ReplyAsync(string.Format("Ping送信に失敗。({0})", reply.Status));
                }

                p.Dispose();
            }
        }


        [Command("weather")]
        [Summary("天候情報を取得します")]
        public async Task GetWeather([Summary("取得したい日にち")] string day = null)
        {
            if (day == null)
            {
                await ReplyAsync("weatherコマンドの後に取得したい日にちを続けて入力してください\n" +
                                 "例:「!weather 今日」");
            }
            else
            {
                var weather = new Weather();
                await ReplyAsync(weather.GetWeatherText(day));
            }
        }

        [Command("dchat")]
        [Alias("c", "chat")]
        [Summary("ドコモAPIを使用して対話します")]
        public async Task DocomoChat([Summary("チャット内容")] string word = null)
        {
            var chat = new ChatAI();
            var response = chat.GetChat(word, Context.Message.Author.Username);
            await ReplyAsync(response.utt);
        }

        [Command("dqa")]
        [Summary("ドコモAPIを使用して質問をします")]
        public async Task DocomoQA([Summary("質問内容")] string question = null)
        {
            if (question == null)
            {
                await ReplyAsync("dqaコマンドの後に質問したい内容を続けて入力してください\n" +
                                 "例:「!dqa 東京スカイツリーの高さは?」");
            }
            else
            {
                var docomo = new DocomoService();
                await ReplyAsync(docomo.GetQA(question));
            }
        }

        [Command("translator")]
        [Alias("t", "trans")]
        [Summary("翻訳します")]
        public async Task Translator([Summary("英語")]string english = null)
        {
            var authtoken = new API.Translator.AzureAuthToken(ini["Azure","Token"]);
            var translate = new API.Translator.Translate();
            var token = authtoken.GetAccessToken();
            var text = translate.TranslateMethod(token, english);
            await ReplyAsync(text);
        }

        [Command("quot")]
        [Summary("名言を返します。")]
        public async Task Quot()
        {
            var quot = new Quotation();
            await ReplyAsync(quot.GetQuot());
        }

        [Command("news")]
        [Summary("ニュースを取得します")]
        public async Task News()
        {
            var news = new News();
            await ReplyAsync(news.GetNews());
        }

        [Command("question")]
        [Summary("アンケートを開始します。（未実装）")]
        public async Task Question([Summary("アンケート内容")] string title = null, string question = null, string img = null)
        {
            var userInfo = Context.User;
            var split = question.Split(',');

            if (img == null)
                img = userInfo.GetAvatarUrl() ?? "http://ravegames.net/ow_userfiles/themes/theme_image_22.jpg";

            Console.WriteLine(img);
            var eb = new EmbedBuilder()
            {
                Color = new Color(0, 255, 0),
                ThumbnailUrl = img,
                Title = $"{title}",
                Description = $"{string.Join(":one: \n", split)}",
                Footer = new EmbedFooterBuilder()
                {
                    Text = $"質問者: {Context.User.Username}#{Context.User.Discriminator}",
                    IconUrl = new Uri(Context.User.GetAvatarUrl()).ToString()
                }
            };

            var m = (RestUserMessage)await Context.Channel.GetMessageAsync(Context.Channel.Id);
            //          await m.AddReactionAsync(new Emoji(EFirst));
            await Context.Channel.SendMessageAsync("", embed: eb);
        }


        [Command("userinfo")]
        [Summary("ユーザの情報を返します")]
        [Alias("user")]
        public async Task UserPerms([Summary("検索ユーザ名")] IUser user = null)
        {
            var userInfo = user ?? Context.User;
            string permissions = "";
            var invoker = Context.User as IGuildUser;
            (userInfo as SocketGuildUser)?.GuildPermissions.ToList()
                .ForEach(x => { permissions += x.ToString() + " , "; });

            var avatarURL = userInfo.GetAvatarUrl() ?? "http://ravegames.net/ow_userfiles/themes/theme_image_22.jpg";

            var eb = new EmbedBuilder()
            {
                Color = new Color(4, 97, 247),
                ThumbnailUrl = new Uri(avatarURL).ToString(),
                Title = $"{userInfo.Username}",
                Description = $"初参加日{userInfo.CreatedAt.ToString().Remove(userInfo.CreatedAt.ToString().Length - 6)}." +
                              $"{(int)(DateTime.Now.Subtract(userInfo.CreatedAt.DateTime).TotalDays)} 日\n\n {permissions}",
                Footer = new EmbedFooterBuilder()
                {
                    Text =
                        $"Requested by {Context.User.Username}#{Context.User.Discriminator} | {userInfo.Username} ID: {userInfo.Id}",
                    IconUrl = new Uri(Context.User.GetAvatarUrl()).ToString()
                }
            };

            await ReplyAsync("", embed: eb);
        }

        [Command("test")]
        [Summary("テスト")]
        public async Task Test()
        {
            await ReplyAsync(new API.PaizaIO().GetCodeOut("b36V-DRQhLmPnxi2oNMtKg"));
        }

    }
}