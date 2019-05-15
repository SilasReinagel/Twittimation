using System;
using System.Collections.Generic;
using System.Text;
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

        public ListTasks(AppDataJsonIo io)
        {
            _io = io;
        }

        public override void Go(string[] args)
        {
            Console.WriteLine("ID".PadRight(16) + "Next Time");
            foreach(var task in _io.LoadOrDefault(Program.TasksFileName, () => new List<ScheduledTask>()))
                Console.WriteLine(task.Id.PadRight(15) + " " + (task.ScheduledOperations[task.CompletedOperations]).ToString());
        }
    }
}
