using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyGame.Engine.Utils
{
    public class Logger
    {
        private const int ERROR_LEVEL = 0;
        private const int WARN_LEVEL = 1;
        private const int INFO_LEVEL = 2;
        private const int DEBUG_LEVEL = 3;
        private const int TRACE_LEVEL = 4;

        //0=error, 1=warn, 2=info, 3=debug, 4=trace
        private static int loggingLevel = 4;
        private static ConcurrentQueue<String> linesToWrite = new ConcurrentQueue<String>();
        private static Semaphore signal = new Semaphore(0, Int32.MaxValue);
        private static Thread fileWriterThread;
        private static volatile bool continueRunning = true;
        private static void StartWriterThread()
        {
            if(fileWriterThread == null || !fileWriterThread.IsAlive)
            {
                continueRunning = true;
                fileWriterThread = new Thread(new ThreadStart(Writer));
                fileWriterThread.Start();
            }
        }

        private static void Writer()
        {
            Process currentProcess = Process.GetCurrentProcess();
            StreamWriter logFile = File.AppendText(DateTime.Now.ToString("MM-dd-yyyy-HH_mm_ss_") + currentProcess.ProcessName + ".log.txt");

            String line;
            while (continueRunning)
            {
                signal.WaitOne();
                if (linesToWrite.TryDequeue(out line))
                {
                    logFile.WriteLine(line);
                    logFile.Flush();
                }
            }

            
            while (linesToWrite.TryDequeue(out line))
            {
                logFile.WriteLine(line);
            }
            logFile.Flush();
            logFile.Close();
        }

        public static void JoinWriter()
        {
            continueRunning = false;
            signal.Release();
            fileWriterThread.Join();
        }

        public static void SetLevelToError()
        {
            loggingLevel = ERROR_LEVEL;
        }

        public static void SetLevelToWarn()
        {
            loggingLevel = WARN_LEVEL;
        }

        public static void SetLevelToInfo()
        {
            loggingLevel = INFO_LEVEL;
        }

        public static void SetLevelToDebug()
        {
            loggingLevel = DEBUG_LEVEL;
        }

        public static void SetLevelToTrace()
        {
            loggingLevel = TRACE_LEVEL;
        }

        private Type type;

        public Logger(Type type)
        {
            this.type = type;
            StartWriterThread();
    
        }

        private void Log(String levelPrefix, String line)
        {
            linesToWrite.Enqueue("[" + levelPrefix + "] [" + this.type.FullName + "] [" + DateTime.Now.ToString("HH:mm:ss:fffffff") + "] " + line);
            signal.Release();
        }

        public void Error(String message)
        {
            this.Log("ERROR", message);
        }

        public void Warn(String message)
        {
            if (loggingLevel >= WARN_LEVEL)
            {
                this.Log("WARN", message);
            }
        }

        public void Info(String message)
        {
            if (loggingLevel >= INFO_LEVEL)
            {
                this.Log("INFO", message);
            }
        }

        public void Debug(String message)
        {
            if (loggingLevel >= DEBUG_LEVEL)
            {
                this.Log("DEBUG", message);
            }
        }

        public void Trace(String message)
        {
            if (loggingLevel >= TRACE_LEVEL)
            {
                this.Log("TRACE", message);
            }
        }
    }
}
