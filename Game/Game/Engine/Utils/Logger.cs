using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyGame.Engine.Utils
{
    public class Logger
    {
        private static ConcurrentQueue<String> linesToWrite = new ConcurrentQueue<String>();
        private static Thread fileWriterThread;

        private static void StartWriterThread()
        {
            if(fileWriterThread == null || !fileWriterThread.IsAlive)
            {
                fileWriterThread = new Thread(new ThreadStart(Writer));
                fileWriterThread.Start();
            }
        }

        private static void Writer()
        {
            StreamWriter w = File.AppendText("log.txt");
            w.Flush();
            w.Close();

            Console.WriteLine(File.Exists("log.txt"));
        }

        public static void JoinWriter()
        {
            fileWriterThread.Join();
        }

        private Type type;

        public Logger(Type type)
        {
            this.type = type;
            StartWriterThread();
    
        }
    }
}
