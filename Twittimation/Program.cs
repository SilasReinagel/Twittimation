using System;
using System.IO;
using Twittimation.Commands;
using Twittimation.IO;

namespace Twittimation
{
    public class Program
    {
        private static AppDataJsonIo _io { get; } = new AppDataJsonIo("Twittimation");

        public static void Main(string[] args)
        {
            var cli = new Cli("Invalid Command, type help to display all commands with their help sections");
            AddCommands(cli);
            if (args.Length == 0)
                cli.Execute("RunSchedule");
            else if (args[0].Equals("debug", StringComparison.OrdinalIgnoreCase))
                while (true)
                    cli.Execute(Console.ReadLine());
            else
                cli.Execute(args, true);
            
        }

        private static void AddCommands(Cli cli)
        {
            var tasksFileName = "Tasks";
            var twitterCredentialsFileName = "Credentials";
            var tweet = new Tweet(_io, twitterCredentialsFileName);
            cli.AddCommands(new SaveTwitterCredentials(_io, twitterCredentialsFileName),
                new ScheduleTweet(_io, tasksFileName, tweet),
                new ScheduleTweetCollection(_io, tasksFileName, tweet),
                new RunSchedule(_io, tasksFileName, cli),
                new ListTasks(_io, tasksFileName),
                new CancelTask(_io, tasksFileName),
                tweet,
                new Help(cli.Commands));
        }
    }
}
