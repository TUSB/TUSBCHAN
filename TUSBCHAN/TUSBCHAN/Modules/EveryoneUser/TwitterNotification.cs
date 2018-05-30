using Discord.Commands;
using System.Threading.Tasks;
using TUSBCHAN.Functions;

namespace TUSBCHAN.Modules.EveryoneUser
{
    public class TwitterNotification : ModuleBase
    {
        public class LoginNotification : ModuleBase
        {
            [Command("addtwitter")]
            [Summary("ログイン通知を追加します")]
            public async Task AddLoginNotification(string twitterId, string english = "false")
            {
                var isEnglish = english == "true" ? true : false;
                var channelId = Context.Channel.Id;
                var sql = string.Format("SELECT * FROM Twitter通知 WHERE チャンネルID = '{0}' AND TwitterID = '{1}'",
                    channelId,
                    twitterId);

                var table = DBAccesser.RunSQLGetResult(sql);

                if (table.Rows.Count > 0)
                {
                    sql = string.Format("UPDATE Twitter通知 SET 英語 = '{0}' WHERE チャンネルID = '{1}', TwitterID = '{2}'",
                    isEnglish,
                    channelId,
                    twitterId);

                    if (DBAccesser.RunSQL(sql))
                    {
                        if (isEnglish)
                        {
                            await ReplyAsync(string.Format("このチャンネルで「{0}」さんのツイートを日本語に翻訳して通知します。",
                                twitterId));
                        }
                        else
                        {
                            await ReplyAsync(string.Format("このチャンネルで「{0}」さんのツイートを通知します。",
                                twitterId));
                        }
                    }
                    else
                    {
                        await ReplyAsync("Twitter通知を登録できませんでした");
                    }
                }
                else
                {
                    sql = string.Format("INSERT INTO Twitter通知 VALUES ('{0}', '{1}', '{2}')",
                    channelId,
                    twitterId,
                    isEnglish);

                    if (DBAccesser.RunSQL(sql))
                    {
                        if (isEnglish)
                        {
                            await ReplyAsync(string.Format("このチャンネルで「{0}」さんのツイートを日本語に翻訳して通知します。",
                                twitterId));
                        }
                        else
                        {
                            await ReplyAsync(string.Format("このチャンネルで「{0}」さんのツイートを通知します。",
                                twitterId));
                        }
                    }
                    else
                    {
                        await ReplyAsync("Twitter通知を登録できませんでした");
                    }
                }
            }

            [Command("removetwitter")]
            [Summary("ログイン通知を削除します")]
            public async Task RemoveLoginNotification(string twitterId)
            {
                var channelId = Context.Channel.Id;
                var sql = string.Format("DELETE FROM Twitter通知 WHERE チャンネルID = '{0}' AND TwitterID = '{1}'",
                    channelId,
                    twitterId);

                if (DBAccesser.RunSQL(sql))
                {
                    await ReplyAsync(string.Format("このチャンネルでの「{0}」さんのツイート通知を停止しました",
                        twitterId));
                }
                else
                {
                    await ReplyAsync(string.Format("このチャンネルでの「{0}」さんのツイート通知を停止できませんでした",
                        twitterId));
                }
            }
        }
    }
}
