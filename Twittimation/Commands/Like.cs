using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
                throw new ArgumentException("Credentials have not been setup. Use 'SaveCredentials' to initialize.");

            try
            {
                var client = new OAuthClient("https://api.twitter.com/1.1/",
                    credentials.ConsumerKey, credentials.ConsumerKeySecret, credentials.AccessToken, credentials.AccessTokenSecret);
                var result = JObject.Parse(client.SendRequest("POST", "favorites/create.json",
                    new Dictionary<string, string>() { { "id", args[0] } }, new JsonData()).Result);
                if (result.ContainsKey("errors"))
                {
                    var error = result.GetValue("errors").ToObject<TwitterError[]>();
                    if (error.Length > 0)
                    {
                        Console.Error.WriteLine("Twitter error: ");
                        Console.Error.WriteLine("code: " + error[0].Code.ToString());
                        Console.Error.WriteLine("message: " + error[0].Message);
                    }
                }
                else
                    Console.WriteLine("Success");
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Network error!");
            }
        }
    }
}
