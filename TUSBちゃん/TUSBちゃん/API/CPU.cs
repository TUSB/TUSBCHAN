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

                if (percent >= 99)
                {
                    var chatchannnel = client.GetChannel(290014442748379137) as SocketTextChannel;
                    chatchannnel.SendMessageAsync("あっ死ぬ...");
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
