using System.Collections.Generic;

namespace Twittimation.Commands
{
    public class Exit : Command
    {
        public override string Name { get; } = "Exit";
        public override List<string> RequiredArgs { get; } = new List<string>();
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Exits the program.";
        public override string ExtendedHelp => HelpInfo;

        public override void Go(string[] args)
        {
            Program.Exiting = true;
        }
    }
}
