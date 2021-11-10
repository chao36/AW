using System;

namespace AW.Log
{
    public interface ILogger
    {
        string Tag { get; set; }

        void Log(string message, string method = null, bool ignoreEvent = false);
        
        void Log(Exception ex, string message = null, string method = null, bool ignoreEvent = false);
        
        string View();
    }
}
