using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// dxwang
/// 4638912@qq.com
/// </summary>
namespace NPublic.Dapper
{
    public static partial class Log
    {
        private static readonly object padlock = new object();
        private static  bool writeLog = false;

        static ConcurrentQueue<LogMessage> _que = new ConcurrentQueue<LogMessage>();
       
        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LogLevel
        {
            Debug,
            Info,
            Error,
            Warning, 
            Fatal
        }

        /// <summary>
        /// 日志内容
        /// </summary>
        public class LogMessage
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public LogLevel Level { get; set; }
            public Exception Exception { get; set; }
            public DateTime Addtime { get; set; }
            public string IP { get; set; }
        }

        /// <summary>
        /// 日志写入MQ队列
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="ex">Exception</param>
        /// <param name="message">日志标题</param>
        /// <param name="message">详细提示，如SQL来源</param>
        public static void EnqueueMessage(LogLevel level, Exception ex, string title, string message="" )
        {
            if (CheckBadWord(message)) { level = LogLevel.Warning; }//危险字符，日志等级设为Fatal

            _que.Enqueue(new LogMessage
            {
                Title = title,
                Message = message,
                Level = level,
                Exception = ex,
                Addtime = DateTime.Now,
                IP = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                .FirstOrDefault()?.Address.ToString()
            });

            if (_que.Count > 3000)//队列最多保存的记录数
            {
                _que.TryDequeue(out LogMessage _);
            }
        }

        /// <summary>
        /// 危险字符
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ex"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        static bool CheckBadWord(string message)
        {
            string pattern = @"!|\'|drop table|create|truncate|xp_cmdshell|exec|netlocalgroup|administrator|net user";
            if (message.Length > 300 || Regex.IsMatch(message, pattern, RegexOptions.IgnoreCase))
            {
                return true;
            }
            return false;
        }

        #region 写文件日志
        /// <summary>
        /// 写文件日志
        /// </summary>
        /// <param name="err"></param>
        /// <param name="file"></param>
        static void WriteFile(LogMessage msg)
        {
            string file = msg.Level.ToString();
            DateTime logtime = msg.Addtime;
            string logstr = string.Format("{0} - {1}\r\n{2}:{3}\r\n{4}\r\n\r\n",
                logtime.ToString(),
                msg.IP,
                msg.Title,
                msg.Message,
                msg.Exception.Message.ToString()
                );

            string CurDir = $"{Directory.GetCurrentDirectory()}" + "\\log\\" + file;
            string filename = string.Format("{0}\\{1}.log", CurDir, logtime.ToString("yyyyMMdd"));
            if (!Directory.Exists(CurDir)) { Directory.CreateDirectory(CurDir); }
            if (!System.IO.File.Exists(filename))
            {
                System.IO.FileStream f = System.IO.File.Create(filename);
                f.Close();
            }
            StreamWriter sw = new StreamWriter(filename, true, Encoding.GetEncoding("UTF-8"));
            sw.Write(logstr);
            sw.Flush();
            sw.Close();
        }
        #endregion
    }
}
