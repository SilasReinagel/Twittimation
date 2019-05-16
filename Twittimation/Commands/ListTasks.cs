using System;
using System.Collections.Generic;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public class ListTasks : Command
    {
        public override string Name { get; } = "ListTasks";
        public override List<string> RequiredArgs { get; } = new List<string>();
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Displays all scheduled tasks with their id and the next time they will have another step executed.";
        public override string ExtendedHelp { get; } = "Displays all scheduled tasks with their id and the next time they will have another step executed.";

        private AppDataJsonIo _io;
        private string _tasksFileName;

        public ListTasks(AppDataJsonIo io, string tasksFileName)
        {
            _io = io;
            _tasksFileName = tasksFileName;
        }

        public override void Go(string[] args)
        {
            Console.WriteLine("ID".PadRight(16) + "Next Time");
            foreach(var task in _io.LoadOrDefault(_tasksFileName, () => new List<ScheduledTask>()))
                Console.WriteLine(task.Id.ToString().PadRight(15) + " " + (task.ScheduledOperations[task.CompletedOperations]).ToString());
        }
    }
}
