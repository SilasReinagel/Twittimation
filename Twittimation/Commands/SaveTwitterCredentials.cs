using System;
using System.Collections.Generic;
using System.Text;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public class SaveTwitterCredentials : Command
    {
        public override string Name { get; } = "SaveCredentials";
        public override List<string> RequiredArgs { get; } = new List<string>()
            { "ConsumerKey", "ConsumerKeySecret", "AccessToken", "AccessTokenSecret" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Saves your keys and tokens needed for oauth authentication for twitter so this program can execute twitter actions on your behalf.";
        public override string ExtendedHelp { get; } = "Saves your keys and tokens needed for oauth authentication for twitter so this program can tweet or do other twitter actions on your behalf.\r\nTo obtain keys and tokens you will need to add an app at developer.twitter.com .";

        private AppDataJsonIo _io;

        public SaveTwitterCredentials(AppDataJsonIo io)
        {
            _io = io;
        }

        public override void Go(string[] args)
        {
            ValidateArgCount(args);
            _io.Save(Program.TwitterCredentialsFileName, new Credentials(args[0], args[1], args[2], args[3]));
        }
    }
}
