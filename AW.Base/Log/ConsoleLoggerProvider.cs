using System;
using System.Collections.Generic;

namespace AW.Log
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public void Log(string message)
        {
            lock (typeof(Console))
            {
                var save = Console.ForegroundColor;

                if (message.Contains("[ERROR]"))
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(message);

                Console.ForegroundColor = save;
            }
        }

        public string View()
        {
            return null;
        }

        public ILogger GetLogger(string tag = null)
        {
            return Logger.New(new List<ILoggerProvider>
            {
                this
            }, tag);
        }
    }
}
