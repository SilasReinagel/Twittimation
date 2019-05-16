using System;
using System.Collections.Generic;
using System.Linq;
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
        private string _tasksFileName;

        public CancelTask(AppDataJsonIo io, string tasksFileName)
        {
            _io = io;
            _tasksFileName = tasksFileName;
        }

        public override void Go(string[] args)
        {
            ValidateArgCount(args);
            var tasks = _io.LoadOrDefault(_tasksFileName, () => new List<ScheduledTask>());
            foreach(var arg in args)
                if (tasks.Any(t => t.Id.ToString() == arg))
                    tasks.RemoveAll(t => t.Id.ToString() == arg);
                else
                    Console.Error.WriteLine("Task with id " + arg + " doesn't exist.");
            _io.Save(_tasksFileName, tasks);
        }
    }
}
