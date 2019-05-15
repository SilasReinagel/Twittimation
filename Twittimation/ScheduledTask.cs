using System;
using System.Collections.Generic;
using System.Text;

namespace Twittimation
{
    public class ScheduledTask
    {
        public string Id { get; set; }
        public List<ScheduledOperation> ScheduledOperations { get; set; }
        public int CompletedOperations { get; set; }

        private ScheduledTask() { }

        public ScheduledTask(string id, ScheduledOperation operation) : this(id, new List<ScheduledOperation>() { operation }) { }

        public ScheduledTask(string id, List<ScheduledOperation> operations)
        {
            Id = id;
            ScheduledOperations = operations;
            CompletedOperations = 0;
        }
    }
}
