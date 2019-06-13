using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Carvana;
using Twittimation.Twitter;

namespace Twittimation.Commands
{
    public sealed class Tweet : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>() { "Text" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Posts a tweet with the specified text.";
        public override string ExtendedHelp { get; } = "Posts a tweet with the specified text using the saved credentials.";

        private readonly ITwitterGateway _twitter;
        private readonly ILog _log;
        
        public Tweet(ITwitterGateway twitter, ILog log)
        {
            _twitter = twitter;
            _log = log;
        }

        protected override void Go(string[] args)
        {
            var tweetContent = args[0];
            if (!TweetLengthIsValid(tweetContent))
                throw new ArgumentOutOfRangeException("Tweet Length is invalid.");
            _twitter.Tweet(tweetContent).GetAwaiter().GetResult()
                .OnFailure(x => _log.Error($"Failed to Post Tweet: {x.ErrorMessage}"))
                .OnSuccess(() => _log.Info($"Successfully Tweeted: {args[0]}"));
        }

        private bool TweetLengthIsValid(string tweetContent)
        {
            var effectiveLength = WithTwitterUrls(tweetContent).Length;
            return effectiveLength > 0 && effectiveLength <= 280;
        }

        private string WithTwitterUrls(string tweetContent)
            => Regex.Replace(tweetContent, @"(http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?", "-----23-Characters-----");
    }
}
