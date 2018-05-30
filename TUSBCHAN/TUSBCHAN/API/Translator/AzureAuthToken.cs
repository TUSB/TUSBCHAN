using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TUSBCHAN.API.Translator
{
    class AzureAuthToken
    {
        // 認証サービスのURL
        private static readonly Uri ServiceUrl = new Uri("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
        // Subscription Keyを渡すときの要求ヘッダー
        private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
        // 認証トークンの有効時間：5分
        private static readonly TimeSpan TokenCacheDuration = new TimeSpan(0, 5, 0);
        // 有効な認証トークンを格納
        private string storedTokenValue = string.Empty;
        // 有効な認証トークンの取得時間
        private DateTime storedTokenTime = DateTime.MinValue;

        /*
          Subscription Keyの取得
        */
        public string SubscriptionKey { get; private set; } = string.Empty;

        /*
         認証サービスへのリクエスト時のHTTPステータスコードの取得
        */
        public HttpStatusCode RequestStatusCode { get; private set; }

        /*
          認証トークンを取得するためのクライアント作成
          <param name="key">Subscription Key</param>
        */
        public AzureAuthToken(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "Subscription Keyが必要です。");
            }

            this.SubscriptionKey = key;
            this.RequestStatusCode = HttpStatusCode.InternalServerError;
        }

        /*
          Subscriptionに紐づいた認証トークンの取得 (非同期)
        */
        public async Task<string> GetAccessTokenAsync()
        {
            if (SubscriptionKey == string.Empty) return string.Empty;

            // 認証トークンが有効な場合は有効な認証トークンを返す
            if ((DateTime.Now - storedTokenTime) < TokenCacheDuration)
            {
                return storedTokenValue;
            }

            // 認証トークンを取得
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = ServiceUrl;
                request.Content = new StringContent(string.Empty);
                request.Headers.TryAddWithoutValidation(OcpApimSubscriptionKeyHeader, this.SubscriptionKey);
                client.Timeout = TimeSpan.FromSeconds(2);
                var response = await client.SendAsync(request);
                this.RequestStatusCode = response.StatusCode;
                response.EnsureSuccessStatusCode();
                var token = await response.Content.ReadAsStringAsync();
                storedTokenTime = DateTime.Now;
                storedTokenValue = "Bearer " + token;
                return storedTokenValue;
            }
        }

        /*
          Subscriptionに紐づいた認証トークンの取得 (同期)
        */
        public string GetAccessToken()
        {
            // 認証トークンが有効な場合は有効な認証トークンを返す
            if ((DateTime.Now - storedTokenTime) < TokenCacheDuration)
            {
                return storedTokenValue;
            }

            // 認証トークンを取得
            string accessToken = null;

            var task = Task.Run(async () =>
            {
                accessToken = await GetAccessTokenAsync();
            });

            while (!task.IsCompleted)
            {
                System.Threading.Thread.Yield();
            }

            if (task.IsFaulted)
            {
                throw task.Exception;
            }
            else if (task.IsCanceled)
            {
                throw new Exception("トークンの取得がタイムアウトしました。");
            }

            return accessToken;
        }
    }
}
