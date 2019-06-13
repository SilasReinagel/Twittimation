using System;

namespace Twittimation
{
    public sealed class WithDateTime : ILog
    {
        private readonly ILog _inner;

        public WithDateTime(ILog inner)
        {
            _inner = inner;
        }
        
        public void Write(LogLevel level, string msg)
        {
            _inner.Write(level, $"{DateTime.Now}: {msg}");
        }
    }
}
