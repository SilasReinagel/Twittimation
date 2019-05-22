using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Twittimation.Http;
using Twittimation.IO;
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

        private readonly IStored<Credentials> _credentials;

        public Tweet(IStored<Credentials> credentials)
        {
            _credentials = credentials;
        }

        protected override void Go(string[] args)
        {
            var credentials = _credentials.Get();
            if (!credentials.AreValid)
                throw new UserErrorException("Credentials have not been setup. Use 'SaveCredentials' to initialize.");
            PostTweet(args, credentials);
        }

        private void PostTweet(string[] args, Credentials credentials)
        {
            RethrowExceptionsAsUserError(() =>
                {
                    new TwitterClient(credentials).SendRequest("POST", "statuses/update.json", new Dictionary<string, string>(),
                        new UrlEncodedData(new Dictionary<string, string>() { { "status", args[0] }, { "trim_user", "1" } })).GetAwaiter().GetResult();
                    Console.WriteLine("Success");
                },
                typeof(TwitterException), typeof(TimeoutException), typeof(WebException));
        }
    }
}
