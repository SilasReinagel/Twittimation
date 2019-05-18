using System;
using System.Collections.Generic;
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

        private readonly IStored<Tasks> _tasks;
        private readonly Like _like;

        public ScheduleLike(IStored<Tasks> tasks, Like like)
        {
            _tasks = tasks;
            _like = like;
        }

        protected override void Go(string[] args)
        {
            if (!DateTimeOffset.TryParse(args[0], out DateTimeOffset time))
                throw new ArgumentException("Invalid time format! Time can be formatted as \"Year/Month/Day Hour:Minute:Second\"");
            if (time < DateTimeOffset.Now)
                throw new ArgumentException("Time is in the past!");
            var tweet = args[1];
            _tasks.Update(tasks =>
            {
                var id = 0;
                while (id < tasks.Count && id == tasks[id].Id)
                    id++;
                var task = new ScheduledTask(id, new ScheduledOperation(time, _like.Name, tweet));
                tasks.Add(task);
                Console.WriteLine("Task added:\r\nID: " + task.Id + "\r\nTime: " + task.ScheduledOperations[0].Time.ToString());
                return tasks;
            });
        }
    }
}
