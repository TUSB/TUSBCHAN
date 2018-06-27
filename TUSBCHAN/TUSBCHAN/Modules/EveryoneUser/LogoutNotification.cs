using Discord.Commands;
using System.Threading.Tasks;
using TUSBCHAN.Functions;

namespace TUSBCHAN.Modules.EveryoneUser
{
    public class LogoutNotification : ModuleBase
    {
        [Command("addlogout")]
        [Summary("ログアウト通知を追加します")]
        public async Task AddLogoutNotification(string word)
        {
            var serverId = Context.Guild.Id;
            var channelId = Context.Channel.Id;
            var sql = string.Format("SELECT * FROM ログアウト WHERE サーバーID = '{0}'",
                serverId);

            var table = DBAccesser.RunSQLGetResult(sql);

            if (table.Rows.Count > 0)
            {
                sql = string.Format("UPDATE ログアウト SET チャンネルID = '{0}', 内容 = '{1}' WHERE サーバーID = '{2}'",
                channelId,
                word,
                serverId);

                if (DBAccesser.RunSQL(sql))
                {
                    await ReplyAsync("ログアウト通知を設定しなおしました");
                }
                else
                {
                    await ReplyAsync("ログアウト通知を設定できませんでした");
                }
            }
            else
            {
                sql = string.Format("INSERT INTO ログアウト VALUES ('{0}', '{1}', '{2}')",
                serverId,
                channelId,
                word);

                if (DBAccesser.RunSQL(sql))
                {
                    await ReplyAsync("ログアウト通知を設定しました");
                }
                else
                {
                    await ReplyAsync("ログアウト通知を設定できませんでした");
                }
            }
        }

        [Command("removelogout")]
        [Summary("ログアウト通知を削除します")]
        public async Task RemoveLogoutNotification()
        {
            var serverId = Context.Guild.Id;
            var sql = string.Format("DELETE FROM ログアウト WHERE サーバーID = '{0}'",
                serverId);

            if (DBAccesser.RunSQL(sql))
            {
                await ReplyAsync(string.Format("ログアウト通知を削除しました"));
            }
            else
            {
                await ReplyAsync("ログアウト通知を削除できませんでした");
            }
        }
    }
}
