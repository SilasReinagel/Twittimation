using System;
using Twittimation.Commands;
using Twittimation.IO;
using Twittimation.Twitter;

namespace Twittimation
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var storage = new JsonFileStorage("./data/");
            var twitter = new HttpTwitterClient(new KeyStored<Credentials>(storage, "Credentials", () => Credentials.Invalid));
            
            var interactiveCli = Init(storage, twitter);
            interactiveCli.AddCommand(new Exit());
            var cli = Init(storage, twitter);
            cli.AddCommand(new InteractiveMode(interactiveCli));
            
            if (args.Length == 0)
                cli.Execute(nameof(Run));
            else
                if (!cli.Execute(args).Succeeded())
                    Environment.Exit(1);
        }

        public static Cli Init(IStorage storage, ITwitterGateway twitter)
        {
            var cli = new Cli();
            AddNormalCommands(cli, storage, twitter);
            return cli;
        }
        
        private static void AddNormalCommands(Cli cli, IStorage storage, ITwitterGateway twitter)
        {
            var credentials = new KeyStored<Credentials>(storage, "Credentials", () => Credentials.Invalid);
            var tweetTasks = new KeyStored<Tasks>(storage, "TweetTasks", () => new Tasks());
            var likeTasks = new KeyStored<Tasks>(storage, "LikeTasks", () => new Tasks());
            var automatonData = new KeyStored<RandomLikeAutomatonData>(storage, "AutomatonData", () => new RandomLikeAutomatonData());
            var tweet = new Tweet(twitter);
            var like = new Like(twitter);
            cli.AddCommands(
                new SaveCredentials(credentials),
                new ScheduleTweet(tweetTasks, tweet),
                new ScheduleTweetCollection(tweetTasks, tweet),
                new Run(tweetTasks, likeTasks, cli, new RandomLikeAutomaton(automatonData, likeTasks, like, twitter)),
                new ListTasks(tweetTasks),
                new Cancel(tweetTasks),
                tweet,
                like,
                new SetMaxLikes(automatonData, twitter),
                new Help(cli.Commands));
        }
    }
}
