using System;
using System.Collections.Generic;
using System.Text;
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

        private Cli _cli;
        private AppDataJsonIo _io;

        public RunSchedule(AppDataJsonIo io, Cli cli)
        {
            _cli = cli;
            _io = io;
        }

        public override void Go(string[] args)
        {
            ValidateArgCount(args);
            Console.WriteLine("Started, exit to stop");
            while (true)
            {
                var time = DateTimeOffset.Now;
                var tasks = _io.LoadOrDefault(Program.TasksFileName, () => new List<ScheduledTask>());
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
                _io.Save(Program.TasksFileName, tasks);
                Thread.Sleep(5000);
            }
        }
    }
}
