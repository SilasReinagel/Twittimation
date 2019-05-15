using System;

namespace Twittimation
{
    public class ScheduledOperation
    {
        public DateTimeOffset Time { get; set; }
        public string Operation { get; set; }

        public ScheduledOperation(DateTimeOffset time, string operation)
        {
            Time = time;
            Operation = operation;
        }
    }
}