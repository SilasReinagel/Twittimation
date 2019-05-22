using System;
using System.Collections.Generic;

namespace Twittimation.Commands
{
    public sealed class InteractiveMode : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>();
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Allows you to use any number of commands without repeatedly opening the program.";
        public override string ExtendedHelp => HelpInfo;

        private Cli _cli;

        public InteractiveMode(Cli cli)
        {
            _cli = cli;
        }

        protected override void Go(string[] args)
        {
            while (true)
                _cli.Execute(Console.ReadLine());
        }
    }
}
