using System;
using System.Collections.Generic;
using Carvana;
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

        private readonly IStored<RandomLikeAutomatonData> _data;
        private readonly ITwitterGateway _twitter;

        public SetMaxLikes(IStored<RandomLikeAutomatonData> data, ITwitterGateway twitter)
        {
            _data = data;
            _twitter = twitter;
        }

        protected override void Go(string[] args)
        {
            if (!int.TryParse(args[1], out int amount) || amount < 0)
                throw new UserErrorException("Amount must be a whole number!");

            var username = args[0];
            var userIdResp = _twitter.GetUserId(username).GetAwaiter().GetResult();
            userIdResp
                .OnSuccess(x => UpdateMaxLikesForFollowee(x, amount))
                .OnSuccess(() => Console.WriteLine($"Max has been set for {username}"))
                .OnFailure(x => Console.Error.WriteLine($"Failed to Like Tweet: {x.ErrorMessage}"));
        }

        private void UpdateMaxLikesForFollowee(string userId, int amount)
        {
            _data.Update(d =>
            {
                d.MaxLikesPerFollowee[userId] = amount;
                return d;
            });
        }
    }
}
