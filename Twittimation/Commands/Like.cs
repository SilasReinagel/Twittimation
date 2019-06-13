using System.Collections.Generic;
using Carvana;
using Twittimation.Twitter;

namespace Twittimation.Commands
{
    public sealed class Like : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>() { "TweetId" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Likes the specified tweet.";
        public override string ExtendedHelp => HelpInfo;

        private readonly ITwitterGateway _twitter;
        private readonly ILog _log;
        
        public Like(ITwitterGateway twitter, ILog log)
        {
            _twitter = twitter;
            _log = log;
        }

        protected override void Go(string[] args)
        {
            var tweetId = args[0];
            _twitter.Like(tweetId).GetAwaiter().GetResult()
                .OnFailure(x => _log.Error($"Failed to Like Tweet: {x.ErrorMessage}"))
                .OnSuccess(() => _log.Info("Successfully Liked Tweet"));   
        }
    }
}
