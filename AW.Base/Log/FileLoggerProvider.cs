using System;
using System.Collections.Generic;
using System.IO;

namespace AW.Log
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private string FilePath { get; set; }

        public FileLoggerProvider(string folder, string name)
        {
            name = name.Replace("{date}", DateTime.Now.ToString("dd.MM.yyyy"));
            FilePath = Path.Combine(folder, name);

            lock (this)
            {
                new DirectoryInfo(folder).Create();

                if (!File.Exists(FilePath))
                    File.Create(FilePath).Close();
            }
        }

        public void Log(string message)
        {
            lock (this)
            {
                using (var stream = new StreamWriter(FilePath, true))
                {
                    stream.WriteLine(message);
                }
            }
        }

        public string View()
        {
            lock (this)
            {
                return File.ReadAllText(FilePath);
            }
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
