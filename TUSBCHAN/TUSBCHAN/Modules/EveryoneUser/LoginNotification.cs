using Discord.Commands;
using System.Threading.Tasks;
using TUSBCHAN.Functions;

namespace TUSBCHAN.Modules.EveryoneUser
{
    public class LoginNotification : ModuleBase
    {
        [Command("addlogin")]
        [Summary("ログイン通知を追加します")]
        public async Task AddLoginNotification(string word)
        {
            var serverId = Context.Guild.Id;
            var channelId = Context.Channel.Id;
            var sql = string.Format("SELECT * FROM ログイン WHERE サーバーID = '{0}'",
                serverId);

            var table = DBAccesser.RunSQLGetResult(sql);

            if (table.Rows.Count > 0)
            {
                sql = string.Format("UPDATE ログイン SET チャンネルID = '{0}', 内容 = '{1}' WHERE サーバーID = '{2}'",
                channelId,
                word,
                serverId);

                if (DBAccesser.RunSQL(sql))
                {
                    await ReplyAsync("ログイン通知を設定し直しました");
                }
                else
                {
                    await ReplyAsync("ログイン通知を設定できませんでした");
                }
            }
            else
            {
                sql = string.Format("INSERT INTO ログイン VALUES ('{0}', '{1}', '{2}')",
                serverId,
                channelId,
                word);

                if (DBAccesser.RunSQL(sql))
                {
                    await ReplyAsync("ログイン通知を設定しました");
                }
                else
                {
                    await ReplyAsync("ログイン通知を設定できませんでした");
                }
            }
        }

        [Command("removelogin")]
        [Summary("ログイン通知を削除します")]
        public async Task RemoveLoginNotification()
        {
            var serverId = Context.Guild.Id;
            var sql = string.Format("DELETE FROM ログイン WHERE サーバーID = '{0}'",
                serverId);

            if (DBAccesser.RunSQL(sql))
            {
                await ReplyAsync(string.Format("ログイン通知を削除しました"));
            }
            else
            {
                await ReplyAsync("ログイン通知を削除できませんでした");
            }
        }
    }
}
