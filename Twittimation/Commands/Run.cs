﻿using System;
using System.Collections.Generic;
using System.Threading;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public sealed class Run : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>();
        public override List<string> OptionalArgs { get; } = new List<string> { "{Forever|Once}"};
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Runs uncompleted tasks that are scheduled for present or the past.";
        public override string ExtendedHelp => HelpInfo;

        private readonly IStored<ScheduledTasks> _tweetTasks;
        private readonly IStored<ScheduledTasks> _likeTasks;
        private readonly Cli _cli;
        private readonly IAutomaton _automaton;
        private readonly ILog _log;

        private DateTimeOffset _lastUpdate;

        public Run(IStored<ScheduledTasks> tasks, IStored<ScheduledTasks> likes, Cli cli, IAutomaton automaton, ILog log)
        {
            _tweetTasks = tasks;
            _likeTasks = likes;
            _cli = cli;
            _automaton = automaton;
            _log = log;
        }

        protected override void Go(string[] args)
        {
            var shouldRunContinuously = args.Length == 0 || !args[0].Equals("once", StringComparison.InvariantCultureIgnoreCase);
            _lastUpdate = DateTimeOffset.Now;
            _log.Info($"Running. Currently, there are {_tweetTasks.Get().Count} scheduled tasks.");
            while (true)
            {
                var time = DateTimeOffset.Now;
                _tweetTasks.Update(RunScheduledTasks(time));
                _likeTasks.Update(RunScheduledTasks(time));
                _automaton.Update(time - _lastUpdate);
                _lastUpdate = time;
                if (!shouldRunContinuously)
                    return;
                Thread.Sleep(1000);
            }
        }

        private Func<ScheduledTasks, ScheduledTasks> RunScheduledTasks(DateTimeOffset time)
        {
            return tasks =>
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
            };
        }
    }
}
