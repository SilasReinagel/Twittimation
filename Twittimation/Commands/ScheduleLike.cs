using System.Collections.Generic;
using Twittimation.Commands.Converters;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public sealed class ScheduleLike : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>() { "Time", "TweetId" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Likes the specified tweet at the scheduled time.";
        public override string ExtendedHelp { get; } = "Likes the specified tweet at the scheduled time. \r\nTime can be formatted as \"Year/Month/Day Hour:Minute:Second\".";

        private readonly IStored<ScheduledTasks> _tasks;
        private readonly Like _like;
        private readonly ILog _log;

        public ScheduleLike(IStored<ScheduledTasks> tasks, Like like, ILog log)
        {
            _tasks = tasks;
            _like = like;
            _log = log;
        }

        protected override void Go(string[] args)
        {
            var time = DateTimeOffsetConverter.ParseFutureTime(args[0]);
            var tweet = args[1];
            _tasks.Update(tasks =>
            {
                var task = new ScheduledTask(tasks.NextId, new ScheduledOperation(time, _like.Name, tweet));
                tasks.Add(task);
                _log.Info($"Added Task '{task.Id}' - {task.ScheduledOperations[0].Time}");
                return tasks;
            });
        }
    }
}
