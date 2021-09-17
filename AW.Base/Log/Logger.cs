﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AW.Log
{
    public class Logger : ILogger
    {
        #region Static

        public static event Action<string> OnLog;
        
        public static ILogger New(IEnumerable<ILoggerProvider> providers, string tag = null)
        {
            return new Logger(providers, tag);
        }

        private static ILogger DefaultLogger;

        public static ILogger Default()
        {
            if (DefaultLogger != null)
                return DefaultLogger;

            DefaultLogger = New(new List<ILoggerProvider>
            {
                new ConsoleLoggerProvider(),
                new FileLoggerProvider("logs", "common-{date}.log")
            });

            return DefaultLogger;
        }

        #endregion

        public string Tag { get; set; }

        private readonly IEnumerable<ILoggerProvider> Providers;

        internal Logger(IEnumerable<ILoggerProvider> providers, string tag)
        {
            Tag = tag;
            Providers = providers;
        }

        public void Log(string message, [CallerMemberName] string method = null, bool ignoreEvent = false)
            => Log(null, message, method, ignoreEvent);

        public void Log(Exception ex, string message = null, [CallerMemberName] string method = null, bool ignoreEvent = false)
        {
            if (ex != null)
            {
                message = $"{(message.IsNull() ? "" : $"{message}: ")}";

                while (ex != null)
                {
                    message += $"{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";
                    ex = ex.InnerException;
                }
            }

            Log($"[{DateTime.Now:hh:mm:ss}]{(!Tag.IsNull() ? $"{Tag.ToUpper()}" : "")}[{(ex != null ? "ERROR" : "LOG")}] {(!method.IsNull() ? $"{method}() - " : "")}{message}", ignoreEvent);
        }

        private void Log(string message, bool ignoreEvent)
        {
            lock (this)
            {
                if (ignoreEvent)
                    OnLog?.Invoke(message);

                foreach (var provider in Providers)
                    provider.Log(message);
            }
        }

        public string View()
        {
            return Providers
                .Select(p => p.View())
                .FirstOrDefault();
        }
    }
}
