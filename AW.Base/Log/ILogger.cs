using System;
using System.Collections;
using System.Collections.Generic;

namespace AW.Log
{
    /// <summary>
    /// Logger interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Tag for log
        /// </summary>
        string Tag { get; set; }

        /// <summary>
        /// Format message and log
        /// </summary>
        /// <param name="message"></param>
        void Log(string message, string method = null, bool ignoreEvent = false);

        /// <summary>
        /// Format exception and log
        /// </summary>
        /// <param name="message"></param>
        void Log(Exception ex, string message = null, string method = null, bool ignoreEvent = false);

        /// <summary>
        /// Return all provider logs (if can) or null
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> View();
    }
}
