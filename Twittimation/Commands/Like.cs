using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Twittimation.Http;
using Twittimation.IO;
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

        private readonly IStored<Credentials> _credentials;

        public Like(IStored<Credentials> credentials)
        {
            _credentials = credentials;
        }

        protected override void Go(string[] args)
        {
            var credentials = _credentials.Get();
            if (!credentials.AreValid)
                throw new UserErrorException("Credentials have not been setup. Use 'SaveCredentials' to initialize.");
            LikeTweet(args, credentials);
        }

        private void LikeTweet(string[] args, Credentials credentials)
        {
            RethrowExceptionsAsUserError(() =>
                {
                    new TwitterClient(credentials).SendRequest("POST", "favorites/create.json",
                        new Dictionary<string, string>() { { "id", args[0] } }, new JsonData()).GetAwaiter().GetResult();
                    Console.WriteLine("Success");
                },
                typeof(TwitterException), typeof(TimeoutException), typeof(WebException));
        }
    }
}
