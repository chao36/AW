using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace AW.Base.Log
{
    public interface ILogger
    {
        void Log(string message, string tag = null, string method = null, bool ignoreEvent = false);
        void Log(Exception ex, string message = null, string tag = null, string method = null, bool ignoreEvent = false);
    }

    public class Logger : ILogger
    {
        private string LogFileName { get; }
        private string Tag { get; }

        public Logger(string tag = null, string file = "log.txt", long maxLogFileSize = 104857600)
        {
            try
            {
                if (File.Exists(file))
                {
                    long size = new FileInfo(file).Length;
                    if (size > maxLogFileSize)
                        File.Delete(LogFileName);
                }
                else
                    File.Create(file);

                LogFileName = file;
                Tag = tag;
            }
            catch { }
        }

        public void Log(string message, string tag = null, [CallerMemberName] string method = null, bool ignoreEvent = false)
            => Log(null, message, tag, method, ignoreEvent);

        public void Log(Exception ex, string message = null, string tag = null, [CallerMemberName] string method = null, bool ignoreEvent = false)
        {
            if (!string.IsNullOrEmpty(message) && ex != null)
                message = $"{message} {ex.Message}{Environment.NewLine}{ex.StackTrace}";
            else if (string.IsNullOrEmpty(message) && ex != null)
                message = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}";

            tag = $"[{(tag ?? Tag ?? "error").ToUpper()}]: ";

            method = !string.IsNullOrEmpty(method) ? $"{method}() - " : "";

            Log($"{tag}{GetDate()}{method}{message}", ignoreEvent);
        }

        private static string GetDate()
            => $"[{DateTime.Now:dd.MM hh:mm:ss}] - ";

        private void Log(string message, bool ignoreEvent)
            => LoggerHelper.Log(message, ignoreEvent, LogFileName);
    }

    public static class LoggerHelper
    {
        public static event Action<string> OnLog;

        public static ILogger DefaultLogger { get; set; }

        public static void Log(string message, string tag = null, [CallerMemberName] string method = null, bool ignoreEvent = false)
            => DefaultLogger.Log(message, tag, method, ignoreEvent);

        public static void Log(Exception ex, string message = null, string tag = null, [CallerMemberName] string method = null, bool ignoreEvent = false)
            => DefaultLogger.Log(ex, message, tag, method, ignoreEvent);

        internal static void Log(string message, bool ignoreEvent, string file)
        {
            if (!ignoreEvent)
                OnLog?.Invoke(message);

            try
            {
                using (StreamWriter stream = new StreamWriter(file, true))
                {
                    stream.WriteLine(message);
                }
            }
            catch { }
        }
    }
}
