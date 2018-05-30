using Discord.Commands;
using Discord.WebSocket;
using System.Data;
using System.Threading.Tasks;
using TUSBCHAN.Functions;

namespace TUSBCHAN.Modules.EveryoneUser
{
    public class Education : ModuleBase
    {
        public async Task Reply(SocketUserMessage message)
        {
            var channel = message.Channel as SocketGuildChannel;
            var serverId = channel.Guild.Id;
            var word = message.Content;
            var sql = string.Format("SELECT * FROM 自動応答 WHERE サーバーID = '{0}' AND 言葉 = '{1}'",
                serverId,
                word);

            var table = DBAccesser.RunSQLGetResult(sql);
            foreach (DataRow row in table.Rows)
            {
                var reply = row["返事"].ToString();
                await message.Channel.SendMessageAsync(reply);
            }
        }

        [Command("listeducation")]
        [Alias("listedu")]
        [Summary("自動応答のリストを表示します")]
        public async Task ListEducation()
        {
            var serverId = Context.Guild.Id;
            var sql = string.Format("SELECT * FROM 自動応答 WHERE サーバーID = '{0}'",
                serverId);

            var table = DBAccesser.RunSQLGetResult(sql);
            foreach (DataRow row in table.Rows)
            {
                var word = row["言葉"].ToString();
                var reply = row["返事"].ToString();

                await ReplyAsync("言葉:" + word);
                await ReplyAsync("返事:" + reply);
            }
        }

        [Command("addeducation")]
        [Alias("addedu")]
        [Summary("自動応答を追加します")]
        public async Task AddEducation(string word, string reply)
        {
            var serverId = Context.Guild.Id;
            var sql = string.Format("SELECT * FROM 自動応答 WHERE サーバーID = '{0}' AND 言葉 = '{1}'",
                serverId,
                word);

            var table = DBAccesser.RunSQLGetResult(sql);

            if (table.Rows.Count > 0)
            {
                sql = string.Format("UPDATE 自動応答 SET 返事 = '{0}' WHERE サーバーID = '{1}' AND 言葉 = '{2}'",
                reply,
                serverId,
                word);

                if (DBAccesser.RunSQL(sql))
                {
                    await ReplyAsync(string.Format("言葉:「{0}」 返事:「{1}」を覚え直しました",
                        word,
                        reply));
                }
                else
                {
                    await ReplyAsync("うまく覚えられませんでした・・・");
                }
            }
            else
            {
                sql = string.Format("INSERT INTO 自動応答 VALUES ('{0}', '{1}', '{2}')",
                serverId,
                word,
                reply);

                if (DBAccesser.RunSQL(sql))
                {
                    await ReplyAsync(string.Format("言葉:「{0}」 返事:「{1}」を覚えました",
                        word,
                        reply));
                }
                else
                {
                    await ReplyAsync("うまく覚えられませんでした・・・");
                }
            }
        }

        [Command("removeeducation")]
        [Alias("removeedu")]
        [Summary("自動応答を削除します")]
        public async Task RemoveEducation(string word)
        {
            var serverId = Context.Guild.Id;
            var sql = string.Format("DELETE FROM 自動応答 WHERE サーバーID = '{0}' AND 言葉 = '{1}'",
                serverId,
                word);

            if (DBAccesser.RunSQL(sql))
            {
                await ReplyAsync(string.Format("言葉:「{0}」を忘れました",
                    word));
            }
            else
            {
                await ReplyAsync("うまく覚えられませんでした・・・");
            }
        }
    }
}
