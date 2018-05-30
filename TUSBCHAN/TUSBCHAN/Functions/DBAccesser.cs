using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUSBCHAN.Functions
{
    class DBAccesser
    {
        /// <summary>
        /// DB接続先設定を取得
        /// </summary>
        /// <returns></returns>
        private static string GetConnectionString()
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = Common.SQLIP,
                InitialCatalog = "TUSBちゃん",
                IntegratedSecurity = false,
                UserID = "sa",
                Password = Common.SQLPassword,
                ConnectTimeout = 2
            }.ToString();
            return connectionString;
        }

        /// <summary>
        /// 渡したSQLを実行し、変更を与えた行があればtrueを返す
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public static bool RunSQL(string sql)
        {
            var ret = false;

            try
            {
                // DB接続
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        try
                        {
                            command.Connection.Open();
                            if (command.ExecuteNonQuery() > 0)
                            {
                                ret = true;
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            ret = false;
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {

            }

            return ret;
        }

        /// <summary>
        /// 渡したSQLを実行し、取得したデータをすべて返す
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public static DataTable RunSQLGetResult(string sql)
        {
            var ret = new DataTable();

            try
            {
                // DB接続
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        try
                        {
                            adapter.Fill(ret);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            adapter.Dispose();
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {

            }

            return ret;
        }
    }
}
