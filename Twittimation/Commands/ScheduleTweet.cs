using System;
using System.Collections.Generic;
using System.Linq;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public class ScheduleTweet : Command
    {
        public override string Name { get; } = "Schedule";
        public override List<string> RequiredArgs { get; } = new List<string>() { "TaskID", "Time", "Text" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Posts the tweet at the scheduled time.";
        public override string ExtendedHelp { get; } = "Posts a tweet with the specified text at the scheduled time.\r\nTime can be formatted as \"Year/Month/Day Hour:Minute:Second\".";

        private AppDataJsonIo _io;
        private Tweet _tweet;

        public ScheduleTweet(AppDataJsonIo io, Tweet tweet)
        {
            _io = io;
            _tweet = tweet;
        }

        public override void Go(string[] args)
        {
            ValidateArgCount(args);
            var id = args[0];
            if (!DateTimeOffset.TryParse(args[1], out DateTimeOffset time))
                throw new ArgumentException("Invalid time format!");
            if (time < DateTimeOffset.Now)
                throw new ArgumentException("Time is in the past!");
            var text = args[2];
            var tasks = _io.LoadOrDefault(Program.TasksFileName, () => new List<ScheduledTask>());
            if (tasks.Any(t => t.Id == id))
                throw new ArgumentException("There is already a task with that id!");
            var task = new ScheduledTask(id, new ScheduledOperation(time, _tweet.CreateCommandString(args[2])));
            tasks.Add(task);
            _io.Save(Program.TasksFileName, tasks);
            Console.WriteLine("Task added:\r\nID: " + task.Id + "\r\nTime: " + task.ScheduledOperations[0].Time.ToString());
        }
    }
}
