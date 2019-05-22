using System;
using System.Collections.Generic;

namespace Twittimation
{
    public class ScheduledOperation
    {
        public DateTimeOffset Time { get; set; }
        public string[] Operation { get; set; }

        public ScheduledOperation(DateTimeOffset time, params string[] operation)
        {
            Time = time;
            Operation = operation;
        }
    }
}