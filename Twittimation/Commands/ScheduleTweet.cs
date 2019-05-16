using System;
using System.Collections.Generic;
using System.Linq;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public class ScheduleTweet : Command
    {
        public override string Name { get; } = "Schedule";
        public override List<string> RequiredArgs { get; } = new List<string>() { "Time", "Text" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Posts the tweet at the scheduled time.";
        public override string ExtendedHelp { get; } = "Posts a tweet with the specified text at the scheduled time.\r\nTime can be formatted as \"Year/Month/Day Hour:Minute:Second\".";

        private AppDataJsonIo _io;
        private string _tasksFileName;
        private Tweet _tweet;

        public ScheduleTweet(AppDataJsonIo io, string tasksFileName, Tweet tweet)
        {
            _io = io;
            _tasksFileName = tasksFileName;
            _tweet = tweet;
        }

        public override void Go(string[] args)
        {
            ValidateArgCount(args);
            if (!DateTimeOffset.TryParse(args[0], out DateTimeOffset time))
                throw new ArgumentException("Invalid time format! Time can be formatted as \"Year/Month/Day Hour:Minute:Second\"");
            if (time < DateTimeOffset.Now)
                throw new ArgumentException("Time is in the past!");
            var text = args[1];
            var tasks = _io.LoadOrDefault(_tasksFileName, () => new List<ScheduledTask>()).OrderBy(t => t.Id).ToList();
            int id = 0;
            while (id < tasks.Count && id == tasks[id].Id)
                id++;
            var task = new ScheduledTask(id, new ScheduledOperation(time, _tweet.CreateCommandString(text)));
            tasks.Add(task);
            _io.Save(_tasksFileName, tasks);
            Console.WriteLine("Task added:\r\nID: " + task.Id + "\r\nTime: " + task.ScheduledOperations[0].Time.ToString());
        }
    }
}
