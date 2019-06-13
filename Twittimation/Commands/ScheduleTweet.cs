using System.Collections.Generic;
using Twittimation.Commands.Converters;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public sealed class ScheduleTweet : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>() { "Time", "Text" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Posts the tweet at the scheduled time.";
        public override string ExtendedHelp { get; } = "Posts a tweet with the specified text at the scheduled time.\r\nTime can be formatted as \"Year/Month/Day Hour:Minute:Second\".";

        private readonly IStored<ScheduledTasks> _tasks;
        private readonly Tweet _tweet;
        private readonly ILog _log;

        public ScheduleTweet(IStored<ScheduledTasks> tasks, Tweet tweet, ILog log)
        {
            _tasks = tasks;
            _tweet = tweet;
            _log = log;
        }

        protected override void Go(string[] args)
        {
            var time = DateTimeOffsetConverter.ParseFutureTime(args[0]);
            var text = args[1];
            _tasks.Update(tasks =>
            {
                var task = new ScheduledTask(tasks.NextId, new ScheduledOperation(time, _tweet.Name, text));
                tasks.Add(task);
                _log.Info($"Added Task '{task.Id}' - {task.ScheduledOperations[0].Time}");
                return tasks;
            });
        }
    }
}
