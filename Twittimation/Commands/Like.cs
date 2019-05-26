using System;
using System.Collections.Generic;
using Carvana;
using Twittimation.Twitter;

namespace Twittimation.Commands
{
    public sealed class Like : Command
    {
        private readonly ITwitterGateway _twitter;
        
        public override List<string> RequiredArgs { get; } = new List<string>() { "TweetId" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Likes the specified tweet.";
        public override string ExtendedHelp => HelpInfo;

        public Like(ITwitterGateway twitter)
        {
            _twitter = twitter;
        }

        protected override void Go(string[] args)
        {
            var tweetId = args[0];
            _twitter.Like(tweetId).GetAwaiter().GetResult()
                .OnFailure(x => Console.Error.WriteLine($"Failed to Like Tweet: {x.ErrorMessage}"))
                .OnSuccess(() => Console.WriteLine("Successfully Liked Tweet"));   
        }
    }
}
