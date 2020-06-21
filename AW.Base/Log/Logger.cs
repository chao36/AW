using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace AW.Base.Log
{
    public static class Logger
    {
        public static event Action<string> OnLog;

        private const string LogFileName = "__log";

        static Logger()
        {
            try
            {
                if (File.Exists(LogFileName))
                {
                    long size = new FileInfo(LogFileName).Length;
                    if (size > 104857600)
                        File.Delete(LogFileName);
                }
            }
            catch { }
        }

        public static void Log(object obj, [CallerMemberName] string method = null, bool ignoreEvent = false)
            => Log(null, obj.ToString(), method, ignoreEvent);

        public static void Log(string message, [CallerMemberName] string method = null, bool ignoreEvent = false)
            => Log(null, message, method, ignoreEvent);

        public static void Log(Exception ex, string message = null, [CallerMemberName] string method = null, bool ignoreEvent = false)
        {
            if (message != null && ex != null)
                message += $" {ex.Message}";
            else if (ex != null)
                message = ex.Message;

            Log($"{method}() - {message ?? "error"}{(ex != null ? ($"{Environment.NewLine}{ex.StackTrace}") : "")}", ignoreEvent);
        }

        public static void Log(string message, bool ignoreEvent = false)
        {
            if (!ignoreEvent)
                OnLog?.Invoke(message);

            try
            {
                using (StreamWriter stream = new StreamWriter(LogFileName, true))
                {
                    stream.WriteLine($"{GetDate()}: {message}");
                }
            }
            catch { }
        }

        private static string GetDate()
            => $"[{DateTime.Now:dd.MM hh:mm:ss}]";
    }
}
