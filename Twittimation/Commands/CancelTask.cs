using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public class CancelTask : Command
    {
        public override string Name { get; } = "Cancel";
        public override List<string> RequiredArgs { get; } = new List<string>() { "Id" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = "Id";
        public override string HelpInfo { get; } = "Cancels the specified tasks.";
        public override string ExtendedHelp => HelpInfo;

        private AppDataJsonIo _io;

        public CancelTask(AppDataJsonIo io)
        {
            _io = io;
        }

        public override void Go(string[] args)
        {
            ValidateArgCount(args);
            var tasks = _io.LoadOrDefault(Program.TasksFileName, () => new List<ScheduledTask>());
            foreach(var arg in args)
                if (tasks.Any(t => t.Id == arg))
                    tasks.RemoveAll(t => t.Id == arg);
                else
                    Console.Error.WriteLine("Task with id " + arg + " doesn't exist.");
            _io.Save(Program.TasksFileName, tasks);
        }
    }
}
