using System.Collections.Generic;

namespace Twittimation
{
    public class ScheduledTask
    {
        public int Id { get; set; }
        public List<ScheduledOperation> ScheduledOperations { get; set; }
        public int CompletedOperations { get; set; }

        private ScheduledTask() { }

        public ScheduledTask(int id, ScheduledOperation operation) : this(id, new List<ScheduledOperation>() { operation }) { }

        public ScheduledTask(int id, List<ScheduledOperation> operations)
        {
            Id = id;
            ScheduledOperations = operations;
            CompletedOperations = 0;
        }
    }
}
