using System;
using System.Collections.Generic;
using System.Threading;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public sealed class Run : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>();
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Continuously runs uncompleted tasks that are scheduled for present or the past.";
        public override string ExtendedHelp => HelpInfo;

        private readonly IStored<Tasks> _tasks;
        private readonly Cli _cli;

        public Run(IStored<Tasks> tasks, Cli cli)
        {
            _tasks = tasks;
            _cli = cli;
        }

        protected override void Go(string[] args)
        {
            Console.WriteLine($"Running. Currently, there are {_tasks.Get().Count} scheduled tasks.");
            while (true)
            {
                var time = DateTimeOffset.Now;
                _tasks.Update(tasks =>
                {
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
                    return tasks;
                });
                Thread.Sleep(5000);
            }
        }
    }
}
