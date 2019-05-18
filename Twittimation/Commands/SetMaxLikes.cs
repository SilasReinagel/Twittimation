using System;
using System.Collections.Generic;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public sealed class SetMaxLikes : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>() { "Amount" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Sets the max number of likes that each followed account can randomly get each day.";
        public override string ExtendedHelp => HelpInfo;

        IStored<RandomLikeAutomatonData> _data;

        public SetMaxLikes(IStored<RandomLikeAutomatonData> data)
        {
            _data = data;
        }

        protected override void Go(string[] args)
        {
            if (!int.TryParse(args[0], out int amount) || amount < 0)
                throw new ArgumentException("Amount must be a whole number!");
            _data.Update((d) => {
                d.MaxLikesPerFolloweePerDay = amount;
                return d;
            });
            Console.WriteLine("Max has been set");
        }
    }
}
