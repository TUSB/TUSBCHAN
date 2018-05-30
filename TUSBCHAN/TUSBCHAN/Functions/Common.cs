using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUSBCHAN.Functions
{
    class Common
    {
        /// <summary>
        /// 接続先SQLServerIPアドレス
        /// </summary>
        public static string SQLIP { set; get; }

        /// <summary>
        /// 接続先SQLServerパスワード
        /// </summary>
        public static string SQLPassword { set; get; }

        /// <summary>
        /// Discordのトークン
        /// </summary>
        public static string DiscordToken { set; get; }

        /// <summary>
        /// TwitterConsumerKey
        /// </summary>
        public static string TwitterConsumerKey { set; get; }

        /// <summary>
        /// TwitterのConsumerSecret
        /// </summary>
        public static string TwitterConsumerSecret { set; get; }

        /// <summary>
        /// TwitterのAccessToken
        /// </summary>
        public static string TwitterAccessToken { set; get; }

        /// <summary>
        /// TwitterのAccessSecret
        /// </summary>
        public static string TwitterAccessSecret { set; get; }

        /// <summary>
        /// Azureのトークン
        /// </summary>
        public static string AzureToken { set; get; }

        public static readonly string TempFolder = Path.Combine(Environment.CurrentDirectory, "tmp");

        /// <summary>
        /// iniファイルを読み込み、変数へ格納する
        /// </summary>
        public static void IniFileLoad()
        {
            var ini = new Library.IniFileAccesser();
            SQLIP = ini["SQL", "IP"];
            SQLPassword = ini["SQL", "Password"];
            DiscordToken = ini["Discord", "Token"];
            TwitterConsumerKey = ini["Twitter", "ConsumerKey"];
            TwitterConsumerSecret = ini["Twitter", "ConsumerSecret"];
            TwitterAccessToken = ini["Twitter", "AccessToken"];
            TwitterAccessSecret = ini["Twitter", "AccessSecret"];
            AzureToken = ini["Azure", "Token"];
        }
    }
}
