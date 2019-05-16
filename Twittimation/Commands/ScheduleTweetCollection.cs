using System;
using System.Collections.Generic;
using System.Linq;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public class ScheduleTweetCollection : Command
    {
        public override string Name { get; } = "ScheduleCollection";
        public override List<string> RequiredArgs { get; } = new List<string>() { "InitialTime", "Interval", "Text" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = "Text";
        public override string HelpInfo { get; } = "Posts a random tweet at the initial time, and then will post a different random tweet at the interval until all tweets are posted.";
        public override string ExtendedHelp { get; } = "Posts a tweet with one of the specified texts at the initial time, and then will post a tweet with an unused text at the interval until there are no unused texts left.\r\nTime can be formatted as \"Year/Month/Day Hour:Minute:Second\".\r\nTime spans can be formatted as \"Days:Hours:Minutes:Seconds\".";

        private AppDataJsonIo _io;
        private string _tasksFileName;
        private Tweet _tweet;

        public ScheduleTweetCollection(AppDataJsonIo io, string tasksFileName, Tweet tweet)
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
            if (!TimeSpan.TryParse(args[1], out TimeSpan interval))
                throw new ArgumentException("Invalid time span format! Time spans can be formatted as \"Days:Hours:Minutes:Seconds\"");
            if (interval <= TimeSpan.Zero)
                throw new ArgumentException("Time span is not positive!");
            var texts = args.SubArray(2, args.Length - 2);
            var tasks = _io.LoadOrDefault(_tasksFileName, () => new List<ScheduledTask>());
            int id = 0;
            while (id < tasks.Count && id == tasks[id].Id)
                id++;
            RandomizeOrder(texts, (int)time.ToUnixTimeSeconds());
            var task = new ScheduledTask(id,
                texts.Select((t, i) => new ScheduledOperation(time + interval * i, _tweet.CreateCommandString(t))).ToList());
            tasks.Add(task);
            _io.Save(_tasksFileName, tasks);
            Console.WriteLine("Task added:\r\nID: " + task.Id + "\r\nFirst Time: " + task.ScheduledOperations[0].Time.ToString());
        }

        private static void RandomizeOrder<T>(IList<T> tasks, int seed)
        {
            var random = new Random(seed);
            for (var i = tasks.Count() - 1; i > 1; i--)
            {
                var randomIndex = random.Next(i + 1);
                var value = tasks[randomIndex];
                tasks[randomIndex] = tasks[i];
                tasks[i] = value;
            }
        }
    }
}
