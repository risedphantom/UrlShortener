using NLog;

namespace Shortener.Singleton
{
    /// <summary>
    /// Singleton: logging class
    /// </summary>
    static class Logger
    {
        public static NLog.Logger Log { get; }
        public static NLog.Logger Request { get; }

        static Logger()
        {
            Log = LogManager.GetLogger("Global");
            Request = LogManager.GetLogger("Request");
        }
    }
}
