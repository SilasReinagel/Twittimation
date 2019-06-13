using System.Collections.Generic;
using System.Linq;

namespace Twittimation
{
    public sealed class ScheduledTasks : List<ScheduledTask>
    {
        public int NextId => this.Any() 
            ? this.Select(x => x.Id).Max() + 1 
            : 1;
    }
}
