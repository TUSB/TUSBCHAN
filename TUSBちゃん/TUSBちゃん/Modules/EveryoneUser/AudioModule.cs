using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VideoLibrary;

namespace TUSBちゃん.Modules.EveryoneUser
{
    [RequireContext(ContextType.Guild)]
    public class AudioModule : ModuleBase
    {
        /*
        private static IAudioClient client;
        private static Stream output;
        private static List<string> list = new List<string>();

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinChannel()
        {
            IVoiceChannel channel = (Context.Message.Author as IGuildUser)?.VoiceChannel;
            client = channel.ConnectAsync().Result;
            await ReplyAsync("ボイスチャンネルに接続しました");
        }

        [Command("logout", RunMode = RunMode.Async)]
        public async Task OutChannel()
        {
            await client.StopAsync();
            await ReplyAsync("ボイスチャンネルを切断しました");
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task MusicAdd(string file)
        {
            file = file.Replace("/", "\\");
            file = file.Replace("\"", "");
            list.Add(file);
            await ReplyAsync(Path.GetFileNameWithoutExtension(file) + "を再生予約しました");
            if (list.Count == 1)
            {
                await MusicPlay();
            }
        }

        [Command("playlist")]
        public async Task ViewPlayList()
        {
            if (list.Count == 0)
            {
                await ReplyAsync("曲は予約されていません");
            }
            else
            {
                await ReplyAsync($"以下の{list.Count}曲が予約されています");
                for (int i = 0; i < list.Count; i++)
                {
                    await ReplyAsync(Path.GetFileNameWithoutExtension(list[i]));
                }
            }
        }

        public async Task MusicPlay()
        {
            while (list.Count > 0)
            {
                if (list[0].IndexOf("http") == -1)
                {
                    await Context.Channel.SendMessageAsync("次の曲を再生:" + Path.GetFileNameWithoutExtension(list[0]));
                    try
                    {
                        string file = @"tmp\music.mp3";
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                        File.Copy(list[0], file);
                        await SendAsync(file);
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync("フォーマット形式が合わない為曲をスキップします\n" +
                            e.Message);
                    }
                    finally
                    {
                        list.RemoveAt(0);
                    }
                }
                else
                {
                    try
                    {
                        string file = @"tmp\music.mp3";
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                        await Context.Channel.SendMessageAsync("次の曲を再生するためにダウンロードしています");
                        YouTube youtube = YouTube.Default;
                        Video vid = youtube.GetVideo(list[0]);
                        System.IO.File.WriteAllBytes(file, vid.GetBytes());
                        await Context.Channel.SendMessageAsync("次の曲を再生:" + vid.FullName);
                        await SendAsync(file);
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync("フォーマット形式が合わない為曲をスキップします\n" +
                            e.Message);
                    }
                    finally
                    {
                        list.RemoveAt(0);
                    }
                }
            }
        }

        [Command("stop")]
        public async Task MusicStop()
        {
            output.Close();
            await ReplyAsync("ミュージックを停止しました");
        }

        private async Task SendAsync(string path)
        {
            // Create FFmpeg using the previous example
            var ffmpeg = CreateStream(path);
            output = ffmpeg.StandardOutput.BaseStream;
            var discord = client.CreatePCMStream(AudioApplication.Mixed);
            await output.CopyToAsync(discord);
            await discord.FlushAsync();
        }

        private static Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }*/
    }
}