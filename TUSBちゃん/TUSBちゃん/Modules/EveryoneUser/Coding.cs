using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace TUSBちゃん.Modules.EveryoneUser
{
    public class Coding : ModuleBase
    {
        private Library.IniFileWrapper ini = new Library.IniFileWrapper();
        private API.PaizaIO paiza = new API.PaizaIO();

        public async Task Reply(SocketUserMessage message)
        {
            SqlConnection connection = null;

            try
            {
                // DB接続
                if (ConnectDB(ref connection) == false)
                {
                }
                string query = string.Format("SELECT * FROM コード");
                SqlCommand command = new SqlCommand(query, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["コマンド"].ToString() == message.Content)
                        {
                            var channel = message.Channel as SocketTextChannel;
                            var id = reader["ID"].ToString();
                            await channel.SendMessageAsync(paiza.GetCodeOut(id));
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
        
        [Command("codelist")]
        [Summary("登録されているコードの一覧を表示します")]
        public async Task ViewCodes()
        {
            SqlConnection connection = null;

            try
            {
                // DB接続
                if (ConnectDB(ref connection) == false)
                {
                }
                string query = string.Format("SELECT * FROM コード");
                SqlCommand command = new SqlCommand(query, connection);
                var sb = new StringBuilder();
                sb.Append("```");
                sb.Append("コマンド タイトル\n");
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sb.AppendFormat("{0} {1}\n",
                            reader["コマンド"].ToString(), reader["タイトル"].ToString());
                    }
                }
                sb.Append("```");
                await ReplyAsync(sb.ToString());
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

        [Command("addcode")]
        [Summary("Paiza.ioのコードを登録します")]
        public async Task AddCode(string word, string id)
        {
            SqlConnection connection = null;

            try
            {
                // DB接続
                if (ConnectDB(ref connection) == false)
                {
                }
                string serchquery = string.Format("SELECT COUNT(*) FROM コード WHERE コマンド=@コマンド");
                SqlCommand serchcommand = new SqlCommand(serchquery, connection);
                serchcommand.Parameters.Add("@コマンド", SqlDbType.NVarChar);
                serchcommand.Parameters["@コマンド"].Value = word;
                string title = paiza.GetTitle(id);

                int i = (int)serchcommand.ExecuteScalar();
                if (i == 0)
                {
                    string insertquery = @"INSERT INTO コード (コマンド, タイトル, ID) VALUES (@コマンド, @タイトル, @ID)"; ;
                    SqlCommand insertcommand = new SqlCommand(insertquery, connection);
                    insertcommand.Parameters.AddWithValue("@コマンド", word);
                    insertcommand.Parameters.AddWithValue("@タイトル", title);
                    insertcommand.Parameters.AddWithValue("@ID", id);
                    i = insertcommand.ExecuteNonQuery();
                    if (i > 0)
                    {
                        await ReplyAsync(string.Format("コード:「{0}」を登録しました", title));
                    }
                }
                else
                {
                    string updatequery = "UPDATE コード SET コマンド=@コマンド,タイトル=@タイトル,ID=@ID WHERE コマンド=@コマンド";
                    SqlCommand updatecommand = new SqlCommand(updatequery, connection);
                    updatecommand.Parameters.Add(new SqlParameter("@コマンド", word));
                    updatecommand.Parameters.Add(new SqlParameter("@タイトル", title));
                    updatecommand.Parameters.Add(new SqlParameter("@ID", id));
                    i = updatecommand.ExecuteNonQuery();
                    if (i > 0)
                    {
                        await ReplyAsync(string.Format("言葉:「{0}」 返事:「{1}」を登録しなおしました", word, id));
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

        [Command("removecode")]
        [Summary("Paiza.ioのコード登録を削除します")]
        public async Task DeleteCode(string word)
        {
            SqlConnection connection = null;
            
            try
            {
                // DB接続
                if (ConnectDB(ref connection) == false)
                {

                }
                string query = string.Format("DELETE FROM コード WHERE コマンド=@コマンド");
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@コマンド", SqlDbType.NVarChar);
                command.Parameters["@コマンド"].Value = word;
                if (command.ExecuteNonQuery() > 0)
                {
                    await ReplyAsync(string.Format("コマンド:「{0}」を削除しました", word));
                }
                else
                {
                    await ReplyAsync(string.Format("そのコマンドは登録されていませんでした"));
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
