using System;
using System.Collections.Generic;
using System.Net;
using Twittimation.Http;
using Twittimation.IO;
using Twittimation.Twitter;

namespace Twittimation.Commands
{
    public sealed class SetMaxLikes : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>() { "FolloweeUsername",  "Amount" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Sets the max number of likes that the specified followed account can randomly get each day.";
        public override string ExtendedHelp => HelpInfo;

        IStored<RandomLikeAutomatonData> _data;
        IStored<Credentials> _credentials;

        public SetMaxLikes(IStored<RandomLikeAutomatonData> data, IStored<Credentials> credentials)
        {
            _data = data;
            _credentials = credentials;
        }

        protected override void Go(string[] args)
        {
            if (!int.TryParse(args[1], out int amount) || amount < 0)
                throw new UserErrorException("Amount must be a whole number!");
            var credentials = _credentials.Get();
            if (!credentials.AreValid)
                throw new UserErrorException("Credentials have not been setup. Use 'SaveCredentials' to initialize.");
            UpdateMaxLikesForFollowee(GetUserId(args[0], credentials), amount);
            Console.WriteLine("Max has been set");
        }

        private void UpdateMaxLikesForFollowee(long id, int amount)
        {
            _data.Update((d) =>
            {
                d.MaxLikesPerFollowee[id] = amount;
                return d;
            });
        }

        private long GetUserId(string username, Credentials credentials)
        {
            return RethrowExceptionsAsUserError(() =>
                {
                    var result = new TwitterClient(credentials).SendRequest("GET", "users/lookup.json",
                        new Dictionary<string, string>() { { "screen_name", username } }, new JsonData()).GetAwaiter().GetResult();
                    return result.GetValue("id").ToObject<long>();
                },
                typeof(TwitterException), typeof(TimeoutException), typeof(WebException));
        }
    }
}
