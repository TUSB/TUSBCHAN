using CoreTweet;
using CoreTweet.Streaming;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TUSBCHAN.Functions;

namespace TUSBCHAN.API.Twitter
{
    class Streaming
    {
        private Tokens token;
        private DiscordSocketClient _client;

        public Streaming(DiscordSocketClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Twitterのストリーミングを開始し、リプライに反応してDiscordへ投稿します
        /// </summary>
        public void StartStreaming()
        {
            var ini = new Library.IniFileAccesser();
            token = CoreTweet.Tokens.Create(
                Common.TwitterConsumerKey,
                Common.TwitterConsumerSecret,
                Common.TwitterAccessToken,
                Common.TwitterAccessSecret);

            Observe();
        }


        public void Observe()
        {
            Console.WriteLine("Twitterストリーム開始");
            var observable = token.Streaming.UserAsObservable();

            observable.Catch(
                    observable.DelaySubscription(
                        TimeSpan.FromSeconds(10)
                        ).Retry()
                )
                .Repeat()
                .Where((StreamingMessage m) => m.Type == CoreTweet.Streaming.MessageType.Create)
                .Cast<StatusMessage>()
                .Select((StatusMessage m) => m.Status)
                .Subscribe(
                    Next,
                    (Exception ex) => Console.WriteLine(ex),
                    () => Console.WriteLine("終点")
                );
        }

        public void Next(Status status)
        {
            var screenName = status.User.ScreenName; // TUSkyBlock等
            var name = status.User.Name; // TUSB等
            var text = status.Text;// つぶやき内容
            var image = "";
            var id = status.Id;
            if (status.Entities.Media != null)
            {
                image = status.Entities.Media.First().MediaUrlHttps;
            }

            // 自動リプライ応答
            if (text.Contains("@TUSkyBlock") && (!text.Contains("RT ")) && screenName != "TUSkyBlock")
            {
                var str = Docomo.ChatAI.GetChat(text, screenName).utt;
                str = "@" + screenName + " " + str;
                token.Statuses.Update(new { status = str, in_reply_to_status_id = id });
            }


            // つぶやき通知
            var sql = string.Format("SELECT * FROM Twitter通知 WHERE TwitterID = '{0}'",
                screenName);
            var table = DBAccesser.RunSQLGetResult(sql);
            if (table.Rows.Count > 0)
            {
                var title = $"{name}さんのつぶやき";

                if (text.Contains("RT "))
                {
                    title = $"{name}さんのリツイート";
                }

                var eb = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        IconUrl = new Uri(status.User.ProfileImageUrlHttps).ToString(),
                        Name = $"{name}",
                        Url = $"https://twitter.com/{screenName}",
                    },
                    Color = new Color(0x55ACEE),
                    Title = title,
                    Url = new Uri($"https://twitter.com/{screenName}/status/{status.Id}").ToString(),
                    Description = $"{text}",
                    ImageUrl = image,
                    ThumbnailUrl = status.User.ProfileImageUrlHttps,
                    Footer = new EmbedFooterBuilder()
                    {
                        IconUrl = status.User.ProfileImageUrlHttps,
                        Text = $"ID:{screenName} | フォロー：{status.User.FriendsCount} フォロワー：{status.User.FollowersCount}"
                    }
                };
                var channelid = ulong.Parse(table.Rows[0][0].ToString());
                var channel = _client.GetChannel(channelid) as SocketTextChannel;
                channel.SendMessageAsync("", embed: eb);
            }
        }
    }
}
