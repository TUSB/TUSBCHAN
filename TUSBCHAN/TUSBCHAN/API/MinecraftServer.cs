using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUSBCHAN.API
{
    class MinecraftServer
    {
        //Processオブジェクトを作成
        private static System.Diagnostics.Process p;
        private Library.IniFileAccesser ini = new Library.IniFileAccesser();
        private System.IO.StreamWriter sw;

        public void StartServer()
        {
            try
            {
                p = new System.Diagnostics.Process();
                //入力できるようにする
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;

                //非同期で出力を読み取れるようにする
                p.StartInfo.RedirectStandardOutput = true;
                p.OutputDataReceived += p_OutputDataReceived;

                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.CreateNoWindow = true;

                //起動
                p.Start();

                //非同期で出力の読み取りを開始
                p.BeginOutputReadLine();

                sw = p.StandardInput;
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("cd /d c:");
                    sw.WriteLine("cd {0}", ini["Minecraft", "ServerDir"]);
                    sw.WriteLine(ini["Minecraft", "CmdLine"]);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                p.WaitForExit();
                p.Close();
            }
        }

        public bool StopServer()
        {
            //入力のストリームを取得
            System.IO.StreamWriter sw = p.StandardInput;
            bool ret = false;

            try
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine(@"stop");
                    ret = true;
                }
            }
            catch (Exception)
            {
                ret = false;
            }
            finally
            {
                sw.Close();
            }
            return ret;
        }

        //OutputDataReceivedイベントハンドラ
        //行が出力されるたびに呼び出される
        static void p_OutputDataReceived(object sender,
            System.Diagnostics.DataReceivedEventArgs e)
        {
            //出力された文字列を表示する
            Console.WriteLine(e.Data);
        }
    }
}
