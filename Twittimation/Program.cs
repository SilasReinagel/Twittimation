using System;
using System.IO;
using Twittimation.Commands;
using Twittimation.IO;

namespace Twittimation
{
    public class Program
    {
        public static bool Exiting = false;
        private static AppDataJsonIo _io { get; } = new AppDataJsonIo("Twittimation");
        public readonly static string TasksFileName = "Tasks";
        public readonly static string TwitterCredentialsFileName = "Credentials";

        public static void Main(string[] args)
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput(8192)));
            var cli = new Cli("Invalid Command, type help to display all commands with their help sections");
            AddCommands(cli);
            if (args.Length == 0)
                while (!Exiting)
                    cli.Execute(Console.ReadLine());
            else
                cli.Execute(args);
        }

        private static void AddCommands(Cli cli)
        {
            var tweet = new Tweet(_io);
            cli.AddCommands(new SaveTwitterCredentials(_io),
                new ScheduleTweet(_io, tweet),
                new ScheduleTweetCollection(_io, tweet),
                new RunSchedule(_io, cli),
                new ListTasks(_io),
                new CancelTask(_io),
                tweet,
                new Help(cli.Commands),
                new Exit());
        }
    }
}
