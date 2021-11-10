namespace AW.Log
{
    public interface ILoggerProvider
    {
        ILogger GetLogger(string tag = null);

        void Log(string message);
        
        string View();
    }
}
