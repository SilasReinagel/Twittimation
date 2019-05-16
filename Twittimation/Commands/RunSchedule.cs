using System;
using System.Collections.Generic;
using System.Threading;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public class RunSchedule : Command
    {
        public override string Name { get; } = "RunSchedule";
        public override List<string> RequiredArgs { get; } = new List<string>();
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Continuously runs uncompleted tasks that are scheduled for present or the past.";
        public override string ExtendedHelp => HelpInfo;

        private AppDataJsonIo _io;
        private string _tasksFileName;
        private Cli _cli;

        public RunSchedule(AppDataJsonIo io, string tasksFileName, Cli cli)
        {
            _cli = cli;
            _tasksFileName = tasksFileName;
            _io = io;
        }

        public override void Go(string[] args)
        {
            ValidateArgCount(args);
            Console.WriteLine("Started, exit to stop");
            while (true)
            {
                var time = DateTimeOffset.Now;
                var tasks = _io.LoadOrDefault(_tasksFileName, () => new List<ScheduledTask>());
                for (var i = tasks.Count - 1; i >= 0; i--)
                {
                    var task = tasks[i];
                    while (task.ScheduledOperations.Count > task.CompletedOperations
                        && time > task.ScheduledOperations[task.CompletedOperations].Time)
                    {
                        _cli.Execute(task.ScheduledOperations[task.CompletedOperations].Operation);
                        task.CompletedOperations++;
                    }
                    if (task.ScheduledOperations.Count == task.CompletedOperations)
                        tasks.RemoveAt(i);
                }
                _io.Save(_tasksFileName, tasks);
                Thread.Sleep(5000);
            }
        }
    }
}
