using System;
using Twittimation.Commands;
using Twittimation.IO;

namespace Twittimation
{
    public class Program
    {
        private static AppDataJsonStorage _io { get; } = new AppDataJsonStorage("Twittimation");

        public static void Main(string[] args)
        {
            var cli = new Cli("Invalid Command, type 'help' to display all commands with their help sections");
            AddCommands(cli);
            if (args.Length == 0)
                cli.Execute(nameof(Run));
            else
                if (!cli.Execute(args))
                    Environment.Exit(1);
        }

        private static void AddCommands(Cli cli)
        {
            var credentials = new KeyStored<Credentials>(_io, "Credentials", () => new Credentials("N/A", "N/A", "N/A", "N/A"));
            var tweetTasks = new KeyStored<Tasks>(_io, "TweetTasks", () => new Tasks());
            var likeTasks = new KeyStored<Tasks>(_io, "LikeTasks", () => new Tasks());
            var automatonData = new KeyStored<RandomLikeAutomatonData>(_io, "AutomatonData", () => new RandomLikeAutomatonData());
            var tweet = new Tweet(credentials);
            var like = new Like(credentials);
            cli.AddCommands(
                new SaveCredentials(credentials),
                new ScheduleTweet(tweetTasks, tweet),
                new ScheduleTweetCollection(tweetTasks, tweet),
                new Run(tweetTasks, likeTasks, cli, new RandomLikeAutomaton(automatonData, likeTasks, like, credentials)),
                new ListTasks(tweetTasks),
                new Cancel(tweetTasks),
                tweet,
                like,
                new SetMaxLikes(automatonData),
                new Help(cli.Commands),
                new InteractiveMode(cli));
        }
    }
}
