using System.Security.Cryptography.X509Certificates;

namespace Twittimation
{
    public interface ILog
    {
        void Write(LogLevel level, string msg);
    }

    public static class LogExtensions
    {
        public static void Error(this ILog log, string msg) => log.Write(LogLevel.Error, msg);
        public static void Info(this ILog log, string msg) => log.Write(LogLevel.Info, msg);
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Error
    }
}
