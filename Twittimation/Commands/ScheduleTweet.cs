using System;
using System.Collections.Generic;
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

        private readonly IStored<Tasks> _tasks;
        private readonly Tweet _tweet;

        public ScheduleTweet(IStored<Tasks> tasks, Tweet tweet)
        {
            _tasks = tasks;
            _tweet = tweet;
        }

        protected override void Go(string[] args)
        {
            if (!DateTimeOffset.TryParse(args[0], out DateTimeOffset time))
                throw new ArgumentException("Invalid time format! Time can be formatted as \"Year/Month/Day Hour:Minute:Second\"");
            if (time < DateTimeOffset.Now)
                throw new ArgumentException("Time is in the past!");
            var text = args[1];
            _tasks.Update(tasks =>
            {
                var id = 0;
                while (id < tasks.Count && id == tasks[id].Id)
                    id++;
                var task = new ScheduledTask(id, new ScheduledOperation(time, _tweet.CreateCommandString(text)));
                tasks.Add(task);
                Console.WriteLine("Task added:\r\nID: " + task.Id + "\r\nTime: " + task.ScheduledOperations[0].Time.ToString());
                return tasks;
            });
        }
    }
}
