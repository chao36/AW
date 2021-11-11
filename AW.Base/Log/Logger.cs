﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AW.Log
{
    /// <summary>
    /// Implement <see cref="ILogger"/>
    /// </summary>
    public class Logger : ILogger
    {
        #region Static

        /// <summary>
        /// Default folder for logs
        /// </summary>
        public static string DefaultFolder { get; set; } = "logs";

        /// <summary>
        /// Event on log message
        /// </summary>
        public static event Action<string> OnLog;

        /// <summary>
        /// Create new logger
        /// </summary>
        /// <param name="providers"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static ILogger New(IEnumerable<ILoggerProvider> providers, string tag = null)
        {
            return new Logger(providers, tag);
        }

        private static ILogger DefaultLogger;

        /// <summary>
        /// Return single default logger
        /// </summary>
        /// <returns></returns>
        public static ILogger Default()
        {
            if (DefaultLogger != null)
                return DefaultLogger;

            DefaultLogger = New(new List<ILoggerProvider>
            {
                new ConsoleLoggerProvider(),
                new FileLoggerProvider(DefaultFolder, "common-{date}.log")
            });

            return DefaultLogger;
        }

        #endregion

        /// <inheritdoc/>
        public string Tag { get; set; }

        private readonly IEnumerable<ILoggerProvider> Providers;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="providers"></param>
        /// <param name="tag"></param>
        protected Logger(IEnumerable<ILoggerProvider> providers, string tag)
        {
            Tag = tag;
            Providers = providers;
        }

        /// <inheritdoc/>
        public void Log(string message, [CallerMemberName] string method = null, bool ignoreEvent = false)
            => Log(null, message, method, ignoreEvent);

        /// <inheritdoc/>
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
    }
}
