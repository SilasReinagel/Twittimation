using System.Collections.Generic;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public sealed class ViewCredentials : Command
    {
        private readonly IStored<Credentials> _credentials;
        private readonly ILog _log;

        public override List<string> RequiredArgs => new List<string>();
        public override List<string> OptionalArgs => new List<string>();
        public override Optional<string> OptionalRepeatedArg => new Optional<string>();
        public override string HelpInfo => "View saved credentials";
        public override string ExtendedHelp => HelpInfo;
        
        public ViewCredentials(IStored<Credentials> credentials, ILog log)
        {
            _credentials = credentials;
            _log = log;
        }

        protected override void Go(string[] args)
        {
            var credentials = _credentials.Get();
            if (!credentials.AreValid)
                _log.Error("No Credentials are set.");
            else
            {
                _log.Info($"{nameof(credentials.Username)}: {credentials.Username}");
                _log.Info($"{nameof(credentials.ConsumerKey)}: {credentials.ConsumerKey}");
                _log.Info($"{nameof(credentials.ConsumerKeySecret)}: {credentials.ConsumerKeySecret}");
                _log.Info($"{nameof(credentials.AccessToken)}: {credentials.AccessToken}");
                _log.Info($"{nameof(credentials.AccessTokenSecret)}: {credentials.AccessTokenSecret}");
            }
        }
    }
}
