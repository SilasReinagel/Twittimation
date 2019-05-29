using System;

namespace Twittimation
{
    public sealed class ConsoleLog : ILog
    {
        public void Write(LogLevel level, string msg)
        {
            var output = level.Equals(LogLevel.Error) ? Console.Error : Console.Out;
            output.WriteLine(msg);
        }
    }
}
