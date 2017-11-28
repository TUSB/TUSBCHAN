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
using System.Threading.Tasks;

namespace TUSBちゃん.API.Twitter
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
            var ini = new Library.IniFileWrapper();
            token = CoreTweet.Tokens.Create(ini["Twitter", "consumerKey"],
                ini["Twitter", "consumerSecret"],
                ini["Twitter", "accessToken"],
                ini["Twitter", "accessSecret"]);
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
            var screenName = status.User.ScreenName;
            var name = status.User.Name;
            var text = status.Text;
            var image = "";
            if (status.Entities.Media != null)
            {
                image = status.Entities.Media.First().MediaUrlHttps;
            }

            if ((text.IndexOf("@TUSkyBlock") != -1 || name == "TUSB") && text.IndexOf("RT ") == -1)
            {
                var title = name == "TUSB" ? $"{name}のつぶやき" : $"{name}さんからのTwitterリプライ";
                
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
                var chatchannnel = _client.GetChannel(290014442748379137) as SocketTextChannel;
                chatchannnel.SendMessageAsync("", embed: eb);
            }
        }
    }
}
