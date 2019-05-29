using System;
using System.Collections.Generic;
using System.Linq;
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
            var log = new ConsoleLog();
         
            var options = new KeyStored<AppOptions>(storage, nameof(AppOptions), () => new AppOptions()).Get();
            if (options.ShowBirdArt)
                new DrawBirdLogo().Execute();
            
            var interactiveCli = Init(log, storage, twitter);
            interactiveCli.AddCommand(new Exit());
            var cli = Init(log, storage, twitter);
            cli.AddCommand(new InteractiveMode(interactiveCli));
            
            RunApp(args, cli);
        }

        private static void RunApp(string[] args, Cli cli)
        {
            if (args.Length == 0)
                cli.Execute(nameof(Run));
            else if (!cli.Execute(args).Succeeded())
                Environment.Exit(1);
        }

        public static Cli Init(ILog log, IStorage storage, ITwitterGateway twitter)
        {
            var cli = new Cli(log);
            AddNormalCommands(cli, log, storage, twitter);
            return cli;
        }
        
        private static void AddNormalCommands(Cli cli, ILog log, IStorage storage, ITwitterGateway twitter)
        {
            var credentials = new KeyStored<Credentials>(storage, "Credentials", () => Credentials.Invalid);
            var tweetTasks = new KeyStored<Tasks>(storage, "TweetTasks", () => new Tasks());
            var likeTasks = new KeyStored<Tasks>(storage, "LikeTasks", () => new Tasks());
            var automatonData = new KeyStored<RandomLikeAutomatonData>(storage, "AutomatonData", () => new RandomLikeAutomatonData());
            var tweet = new Tweet(twitter);
            var like = new Like(twitter);
            var rawCommands = new List<Command>
            {
                new SaveCredentials(credentials, log),
                new ViewCredentials(credentials, log),
                new ScheduleTweet(tweetTasks, tweet),
                new ScheduleTweetCollection(tweetTasks, tweet),
                new Run(tweetTasks, likeTasks, cli, new RandomLikeAutomaton(automatonData, likeTasks, like, twitter)),
                new ListTasks(tweetTasks),
                new Cancel(tweetTasks),
                tweet,
                like,
                new SetMaxLikes(automatonData, twitter)
            };
            
            cli.AddCommands(rawCommands
                    .Select(x => new WithDisplayedUsername(log, credentials, x))
                    .Concat(new List<ICommand> { new Help(rawCommands.ToDictionary(x => x.Name, x => x)) } )
                    .ToArray());
        }
    }
}
