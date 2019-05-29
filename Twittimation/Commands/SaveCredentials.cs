using System.Collections.Generic;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public sealed class SaveCredentials : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>()
            { "Username", "ConsumerKey", "ConsumerKeySecret", "AccessToken", "AccessTokenSecret" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Saves your keys and tokens needed for oauth authentication for twitter so this program can execute twitter actions on your behalf.";
        public override string ExtendedHelp { get; } = "Saves your keys and tokens needed for oauth authentication for twitter so this program can tweet or do other twitter actions on your behalf.\r\nTo obtain keys and tokens you will need to add an app at developer.twitter.com .";

        private readonly IStored<Credentials> _credentials;
        private readonly ILog _log;

        public SaveCredentials(IStored<Credentials> credentials, ILog log)
        {
            _credentials = credentials;
            _log = log;
        }

        protected override void Go(string[] args)
        {
            _credentials.Update(_ => new Credentials(args[0], args[1], args[2], args[3], args[4]));
            _log.Info($"Saved Credentials for {args[0]}");
        }
    }
}
