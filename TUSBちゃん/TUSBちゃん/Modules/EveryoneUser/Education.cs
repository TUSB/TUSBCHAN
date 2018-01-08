using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUSBちゃん.Modules.EveryoneUser
{
    public class Education : ModuleBase
    {
        private Library.IniFileWrapper ini = new Library.IniFileWrapper();

        public async Task Reply(SocketUserMessage message)
        {
            SqlConnection connection = null;

            try
            {
                // DB接続
                if (ConnectDB(ref connection) == false)
                {
                }
                string query = string.Format("SELECT * FROM 応答内容");
                SqlCommand command = new SqlCommand(query, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (message.Content.Contains(reader["言葉"].ToString()))
                        {
                            var channel = message.Channel as SocketTextChannel;
                            await channel.SendMessageAsync(reader["返事"].ToString());
                        }
                    }
                }
            }
            catch (Exception)
            {
                //データ取得失敗
            }
            finally
            {
                // データベースの接続終了
                connection.Close();
                connection.Dispose();
            }
        }

        [Command("addeducation")]
        [Alias("addedu")]
        [Summary("自動応答を教育します")]
        public async Task AddEducation(string _word, string _reply)
        {
            SqlConnection connection = null;

            try
            {
                // DB接続
                if (ConnectDB(ref connection) == false)
                {
                }
                string serchquery = string.Format("SELECT COUNT(*) FROM 応答内容 WHERE 言葉=@言葉");
                SqlCommand serchcommand = new SqlCommand(serchquery, connection);
                serchcommand.Parameters.Add("@言葉", SqlDbType.NVarChar);
                serchcommand.Parameters["@言葉"].Value = _word;

                int i = (int)serchcommand.ExecuteScalar();
                if (i == 0)
                {
                    string insertquery = @"INSERT INTO 応答内容 (言葉, 返事) VALUES (@言葉, @返事)"; ;
                    SqlCommand insertcommand = new SqlCommand(insertquery, connection);
                    insertcommand.Parameters.AddWithValue("@言葉", _word);
                    insertcommand.Parameters.AddWithValue("@返事", _reply);
                    i = insertcommand.ExecuteNonQuery();
                    if (i > 0)
                    {
                        await ReplyAsync(string.Format("言葉:「{0}」 返事:「{1}」を覚えました", _word, _reply));
                    }
                }
                else
                {
                    string updatequery = "UPDATE 応答内容 SET 言葉=@言葉,返事=@返事 WHERE 言葉=@言葉";
                    SqlCommand updatecommand = new SqlCommand(updatequery, connection);
                    updatecommand.Parameters.Add(new SqlParameter("@言葉", _word));
                    updatecommand.Parameters.Add(new SqlParameter("@返事", _reply));
                    i = updatecommand.ExecuteNonQuery();
                    if (i > 0)
                    {
                        await ReplyAsync(string.Format("言葉:「{0}」 返事:「{1}」を覚え直しました", _word, _reply));
                    }
                }
            }
            catch (Exception)
            {
                //データ取得失敗
            }
            finally
            {
                // データベースの接続終了
                connection.Close();
                connection.Dispose();
            }
        }

        [Command("removeeducation")]
        [Alias("removeedu")]
        [Summary("自動応答を忘却します")]
        public async Task DeleteEducation(string _word)
        {
            SqlConnection connection = null;

            try
            {
                // DB接続
                if (ConnectDB(ref connection) == false)
                {

                }
                string query = string.Format("DELETE FROM 応答内容 WHERE 言葉=@言葉");
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@言葉", SqlDbType.NVarChar);
                command.Parameters["@言葉"].Value = _word;
                if (command.ExecuteNonQuery() > 0)
                {
                    await ReplyAsync(string.Format("言葉:「{0}」を忘れました", _word));
                }
                else
                {
                    await ReplyAsync(string.Format("その言葉は覚えていませんでした"));
                }
            }
            catch (Exception)
            {
                //データ取得失敗
            }
            finally
            {
                // データベースの接続終了
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// DB接続先設定を取得
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = ini["SQL", "IP"],
                InitialCatalog = "TUSBCHAN",
                IntegratedSecurity = false,
                UserID = "sa",
                Password = ini["SQL", "Password"],
                ConnectTimeout = 2
            }.ToString();
            return connectionString;
        }

        /// <summary>
        /// DB接続
        /// </summary>
        /// <param name="connection">接続DB</param> 
        public bool ConnectDB(ref SqlConnection connection)
        {
            bool bRet = false;
            string ErrMsg = "";

            try
            {
                //接続文字列の作成
                string connectionString = GetConnectionString();

                connection = new SqlConnection(connectionString);


                //データベースの接続
                connection.Open();

            }
            catch (Exception ex)
            {
                //接続失敗
                connection = null;
                ErrMsg = ex.Message + "[場所]" + ex.TargetSite;
                Console.WriteLine(ErrMsg);
            }
            finally
            {
                if (ErrMsg == "")
                {
                    bRet = true;
                }
            }

            return bRet;
        }
    }
}
