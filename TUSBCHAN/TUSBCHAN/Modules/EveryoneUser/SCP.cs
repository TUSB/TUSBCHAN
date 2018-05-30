using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TUSBCHAN.Modules.EveryoneUser
{
    public class SCP : ModuleBase
    {
        private string
            _item = "アクセス不許可",
            _oc = "アクセス不許可",
            _protocol = "アクセス不許可",
            _bio = "アクセス不許可",
            _img,
            _ocimg = "https://cdn.discordapp.com/attachments/308673322558947339/351577722876592139/noclass.png",
            _urlstring,
            _rate;

        private uint _colorbar = 0xffffff;

        [Command("scp")]
        [Summary("SCPデータを検索します。")]
        public async Task ScpTask([Summary("オブジェクトナンバー")] string itemNumber = null, string lang = null)
        {
            if (itemNumber == null)
            {
                await ReplyAsync("SCPオブジェクトナンバーを入力してください [例： !scp 173 jp]\n" +
                                 "***国コード*** ```" +
                                 "jp\n" +
                                 "ru\n" +
                                 "ko\n" +
                                 "cn\n" +
                                 "fr\n" +
                                 "es\n" +
                                 "pl\n" +
                                 "th\n" +
                                 "de\n" +
                                 "it\n" +
                                 "```");
            }
            else
            {
                int Number = Convert.ToInt32(itemNumber);
                if (Number <= 99)
                {
                    itemNumber = "0" + itemNumber;
                    if (Number <= 9)
                    {
                        itemNumber = "0" + itemNumber;
                    }
                }
                string url = "http://ja.scp-wiki.net/printer--friendly/";
                string message = "本部";
                switch (lang)
                {
                    case "jp":
                        _urlstring = $"{url}/scp-{itemNumber}-{lang}";
                        message = "日本支部";
                        break;
                    case "ru":
                        _urlstring = $"{url}/scp-{itemNumber}-{lang}";
                        message = "ロシア支部";
                        break;
                    case "ko":
                        _urlstring = $"{url}/scp-{itemNumber}-{lang}";
                        message = "韓国支部";
                        break;
                    case "cn":
                        _urlstring = $"{url}/scp-{itemNumber}-{lang}";
                        message = "中国支部";
                        break;
                    case "fr":
                        _urlstring = $"{url}/scp-{itemNumber}-{lang}";
                        message = "フランス支部";
                        break;
                    case "es":
                        _urlstring = $"{url}/scp-{itemNumber}-{lang}";
                        message = "スペイン支部";
                        break;
                    case "pl":
                        _urlstring = $"{url}/scp-{itemNumber}-{lang}";
                        message = "ポーランド支部";
                        break;
                    case "th":
                        _urlstring = $"{url}/scp-{itemNumber}-{lang}";
                        message = "タイ支部";
                        break;
                    case "de":
                        _urlstring = $"{url}/scp-{itemNumber}-{lang}";
                        message = "ドイツ支部";
                        break;
                    case "it":
                        _urlstring = $"{url}/scp-{itemNumber}-{lang}";
                        message = "イタリア支部";
                        break;
                    default:
                        _urlstring = $"{url}/scp-{itemNumber}";
                        break;
                }


                try
                {
                    IHtmlDocument doc;
                    using (var client = new HttpClient())
                    using (var stream = await client.GetStreamAsync(new Uri(_urlstring)))
                    {
                        var parser = new HtmlParser();
                        doc = await parser.ParseAsync(stream);
                    }

                    doc.QuerySelector("#print-options").Remove();
                    doc.QuerySelector("head").Remove();
                    doc.QuerySelector("#page-title").Remove();
                    _rate = doc.QuerySelector(".number").Text();
                    doc.QuerySelector(".page-rate-widget-box").Remove();

                    _img = doc.QuerySelector(".scp-image-block > img").GetAttribute("src");
                    doc.QuerySelector(".scp-image-block").Remove();


                    _item = $"SCP-{itemNumber}";
                    if (_img == null)
                    {
                        _img =
                            "https://cdn.discordapp.com/attachments/308673322558947339/351561886715740170/SCP.jpg";
                    }
                    var nodeList = doc.QuerySelectorAll("p");
                    foreach (var item in nodeList)
                    {
                        string text = doc.QuerySelector("strong").Text();

                        if (text == "アイテム番号:")
                        {
                            doc.QuerySelector("strong").Remove();
                        }
                        if (text == "オブジェクトクラス:")
                        {
                            doc.QuerySelector("strong").Remove();
                            _oc = item.Text();

                            if (item.Text().LastIndexOf("Keter", StringComparison.Ordinal) == 1)
                            {
                                _ocimg =
                                    "https://cdn.discordapp.com/attachments/308673322558947339/351561874065850369/Keter.png";
                                _colorbar = 0xff0000;
                            }
                            if (item.Text().LastIndexOf("Euclid", StringComparison.Ordinal) == 1)
                            {
                                _ocimg =
                                    "https://cdn.discordapp.com/attachments/308673322558947339/351561876808794112/Euclid.png";
                                _colorbar = 0xffa500;
                            }
                            if (item.Text().LastIndexOf("Safe", StringComparison.Ordinal) == 1)
                            {
                                _ocimg =
                                    "https://cdn.discordapp.com/attachments/308673322558947339/351561880319295488/Safe.png";
                                _colorbar = 0x32cd32;
                            }
                        }
                        if (text == "特別収容プロトコル:")
                        {
                            doc.QuerySelector("strong").Remove();
                            var pro = item.Text();
                            _protocol = pro.Substring(0, pro.IndexOf("。", StringComparison.Ordinal) + 1);
                        }
                        if (text == "説明:")
                        {
                            doc.QuerySelector("strong").Remove();
                            var bi = item.Text();
                            _bio = bi.Substring(0, bi.IndexOf("。", StringComparison.Ordinal) + 1);
                        }
                    }
                    if (_oc.Length > 20)
                    {
                        _oc = "アクセス拒否";
                    }
                    if (_protocol.Length > 200)
                    {
                        _oc = "アクセス拒否";
                    }

                    if (_bio.Length > 200)
                    {
                        _oc = "アクセス拒否";
                    }
                }
                catch (Exception e)
                {
                    _item =
                        _oc =
                            _protocol =
                                _bio = "クリアランスレベル不足";
                    _img = "https://cdn.discordapp.com/attachments/308673322558947339/351561886715740170/SCP.jpg";
                    Console.Write(e);
                    throw;
                }
                finally
                {
                    var eb = new EmbedBuilder()
                    {
                        Author = new EmbedAuthorBuilder()
                        {
                            IconUrl = new Uri(
                                    "https://cdn.discordapp.com/attachments/308673322558947339/351564731292712963/SCP_Logo.png")
                                .ToString(),
                            Name = $"SCP財団 確保 / 収容 / 保護   {message}",
                            Url = "http://ja.scp-wiki.net/",
                        },
                        Color = new Color(_colorbar),
                        ThumbnailUrl =
                            new Uri(_ocimg)
                                .ToString(),
                        ImageUrl = new Uri(_img).ToString(),
                        Url = new Uri($"http://ja.scp-wiki.net/scp-{itemNumber}").ToString(),
                        Title = $"{_item}",
                        Description = $"**評価:** {_rate}",
                    };
                    eb.AddField(efd =>
                    {
                        efd.Name = "アイテム番号";
                        efd.IsInline = true;
                        efd.Value = _item;
                    });

                    eb.AddField(efd =>
                    {
                        efd.Name = "オブジェクトクラス";
                        efd.IsInline = true;
                        efd.Value = _oc;
                    });

                    eb.AddField((efd) =>
                    {
                        efd.Name = "収容プロトコル";
                        efd.IsInline = true;
                        efd.Value = _protocol;
                    });

                    eb.AddField(efd =>
                    {
                        efd.Name = "説明";
                        efd.IsInline = true;
                        efd.Value = _bio;
                    });
                    await ReplyAsync("", embed: eb);
                }
            }
        }
        [Command("info")]
        [Summary("サーバ状態を表示します。")]
        public async Task Info()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            EmbedBuilder eb = new EmbedBuilder();
            string name;
            if (Context.Guild != null)
            {
                var user = await Context.Guild.GetCurrentUserAsync();
                name = user.Nickname ?? user.Username;
            }
            else
                name = Context.Client.CurrentUser.Username;
            eb.Author = new EmbedAuthorBuilder().WithName(name).WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl());
            eb.ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl();
            eb.AddField(x =>
            {
                x.IsInline = false;
                x.Name = "Info";
                x.Value = $"- ライブラリ: Discord.Net ({DiscordConfig.Version})\n" +
                          $"{RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture}\n" +
                          $"- 起動時間: {(DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss")}";
            });

            await ReplyAsync("", false, eb.Build());
        }

        /*
        [Command("announce")]
        [Summary("アラートメッセージの送信をします。")]
        public async Task announce([Summary("メッセージ")] string title = null, string bio = null)
        {
                var userInfo = Context.User; 
                var eb = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        IconUrl = new Uri(Context.User.GetAvatarUrl()).ToString(),
                        Name = $"{Context.User.Username}",
                        Url = "https://twitter.com/wakokara",
                    },
                    Color = new Color(0x00ff80),
                    ThumbnailUrl = "https://pbs.twimg.com/media/DHcy72vUQAEFMqf.jpg",
                    ImageUrl = "https://pbs.twimg.com/media/DHcy72vUQAEFMqf.jpg",
                    Url = "https://twitter.com/wakokara",
                    Title = $"{title}",
                    Description = $"{bio}",
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = $"Requested by {Context.User.Username}#{Context.User.Discriminator} | {userInfo.Username} ID: {userInfo.Id}",
                        IconUrl = "https://pbs.twimg.com/media/DHcy72vUQAEFMqf.jpg"
                    },
          
                };
            eb.AddField((efd) =>
            {
                efd.Name = "LBPtatuharu";
                efd.IsInline = true;
                efd.Value = "さん";
            });
            eb.AddField((efd) =>
            {
                efd.Name = "2110tatuharru";
                efd.IsInline = true;
                efd.Value = "さん";
            });
            eb.AddField((efd) =>
            {
                efd.Name = "Tusbtatuharu";
                efd.IsInline = false;
                efd.Value = "さん";
            });

            await ReplyAsync("",embed: eb);
           
        }*/
    }
}
