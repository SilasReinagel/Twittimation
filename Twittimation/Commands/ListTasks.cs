using System;
using System.Collections.Generic;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public sealed class ListTasks : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>();
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Displays all scheduled tasks with their id and the next time they will have another step executed.";
        public override string ExtendedHelp { get; } = "Displays all scheduled tasks with their id and the next time they will have another step executed.";

        private readonly IStored<Tasks> _tasks;
        
        public ListTasks(IStored<Tasks> tasks)
        {
            _tasks = tasks;
        }

        protected override void Go(string[] args)
        {
            Console.WriteLine("ID".PadRight(16) + "Next Time");
            foreach(var task in _tasks.Get())
                Console.WriteLine(task.Id.ToString().PadRight(16) + task.ScheduledOperations[task.CompletedOperations]);
        }
    }
}
