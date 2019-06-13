using System;
using System.Collections.Generic;
using System.Linq;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public sealed class Cancel : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>() { "TaskId" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = "TaskId";
        public override string HelpInfo { get; } = "Cancels the specified tasks.";
        public override string ExtendedHelp => HelpInfo;

        private readonly IStored<ScheduledTasks> _tasks;
        private readonly ILog _log;

        public Cancel(IStored<ScheduledTasks> tasks, ILog log)
        {
            _tasks = tasks;
            _log = log;
        }

        protected override void Go(string[] args)
        {
            _tasks.Update(tasks =>
            {
                foreach(var arg in args)
                    if (tasks.Any(t => t.Id.ToString() == arg))
                        tasks.RemoveAll(t => t.Id.ToString() == arg);
                    else
                        _log.Error("Task with id " + arg + " doesn't exist.");
                return tasks;
            });
        }
    }
}
