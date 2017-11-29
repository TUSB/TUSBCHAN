using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Threading;

namespace TUSBちゃん.API
{
    class CPU
    {
        private static float percent = 0;
        private static float tmp = 70;

        public static void WatchingStart(DiscordSocketClient client)
        {
            while (true)
            {
                GetCPUPercent();

                if (percent > tmp)
                {
                    tmp = percent;
                    var chatchannnel = client.GetChannel(290014442748379137) as SocketTextChannel;
                    string word = string.Format("録画サーバーのCPU使用率が{0}%を超過しました。\nこのままでは運営に支障が発生します。" +
                        "\nサーバーをご利用の皆様は一度使用を中止してください", percent);
                    chatchannnel.SendMessageAsync(word);
                }
                else if (percent <= 70)
                {
                    tmp = 70;
                }
            }
        }

        private static void GetCPUPercent()
        {
            var pfcounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            percent = pfcounter.NextValue();
            Thread.Sleep(1000);
            percent = pfcounter.NextValue();
        }
    }
}
