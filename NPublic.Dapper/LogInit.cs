using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// dxwang
/// 4638912@qq.com
/// </summary>
namespace NPublic.Dapper
{
    /// <summary>
    /// 日志存盘或入库，发邮件等
    /// </summary>
    public partial class Log
    {
        /// <summary>
        /// 致命错误
        /// </summary>
        /// <param name="msg"></param>
        public static void Fatal(LogMessage msg)
        {
            WriteFile(msg);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="msg"></param>
        public static void Warn(LogMessage msg)
        {
            WriteFile(msg);
        }

        /// <summary>
        /// 一般错误
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(LogMessage msg)
        {
            WriteFile(msg);
        }

        /// <summary>
        /// 调试错误
        /// </summary>
        /// <param name="msg"></param>
        public static void Debug(LogMessage msg)
        {
            WriteFile(msg);
        }

        /// <summary>
        /// 信息提示
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(LogMessage msg)
        {
            WriteFile(msg);
        }

        

        #region  日志处理入口
        /// <summary>
        /// 日志处理入口
        /// 异步，单例
        /// </summary>
        public static void LogInit()
        {
            lock (padlock)
            {
                if (writeLog) { return; }
                Console.WriteLine("LogInit...");

                writeLog = true;
                new Task(() =>
                {
                    while (true)
                    {
                        while (_que.Count > 0 && _que.TryDequeue(out LogMessage msg))
                        {
                            //日志存盘
                            switch (msg.Level)
                            {
                                case LogLevel.Debug:
                                    Debug(msg);
                                    break;
                                case LogLevel.Info:
                                    Info(msg);
                                    break;
                                case LogLevel.Error:
                                    Error(msg);
                                    break;
                                case LogLevel.Warning:
                                    Warn(msg);
                                    break;
                                case LogLevel.Fatal:
                                    Fatal(msg);
                                    break;
                            }
                        }

                        Thread.Sleep(1000);
                    }
                }).Start();
            }
        }
        #endregion
    }
}
