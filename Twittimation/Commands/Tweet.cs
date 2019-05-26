using System;
using System.Collections.Generic;
using Carvana;
using Twittimation.Twitter;

namespace Twittimation.Commands
{
    public sealed class Tweet : Command
    {
        private readonly ITwitterGateway _twitter;
        
        public override List<string> RequiredArgs { get; } = new List<string>() { "Text" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Posts a tweet with the specified text.";
        public override string ExtendedHelp { get; } = "Posts a tweet with the specified text using the saved credentials.";

        public Tweet(ITwitterGateway twitter)
        {
            _twitter = twitter;
        }

        protected override void Go(string[] args)
        {
            var tweetContent = args[0];
            _twitter.Tweet(tweetContent).GetAwaiter().GetResult()
                .OnFailure(x => Console.Error.WriteLine($"Failed to Post Tweet: {x.ErrorMessage}"))
                .OnSuccess(() => Console.WriteLine("Successfully posted Tweet"));
        }
    }
}
