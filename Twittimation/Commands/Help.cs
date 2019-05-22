using System;
using System.Collections.Generic;

namespace Twittimation.Commands
{
    public sealed class Help : Command
    {
        public override List<string> RequiredArgs { get; } = new List<string>();
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = "Command";
        public override string HelpInfo { get; } = "If no commands were specified this writes the help section of all commands, otherwise this writes the extended help section for those commands.";
        public override string ExtendedHelp => HelpInfo;

        private readonly Dictionary<string, Command> _commands;

        public Help(Dictionary<string, Command> commands)
        {
            _commands = commands;
        }
        
        protected override void Go(string[] args)
        {
            if (args.Length == 0)
                foreach(var command in _commands)
                    Console.WriteLine("\t" + command.Value.CreateSyntaxString());
            else
            {
                var invalidArgs = new List<string>();
                foreach (var arg in args)
                {
                    if (_commands.ContainsKey(arg.ToUpper()))
                    {
                        var command = _commands[arg.ToUpper()];
                        Console.WriteLine("\t" + command.CreateSyntaxString());
                        Console.WriteLine("\t" + string.Join("\t\r\n", command.ExtendedHelp.Split("\r\n")));
                    }
                    else
                    {
                        invalidArgs.Add(arg);
                    }
                }
                if (invalidArgs.Count > 0)
                    throw new UserErrorException("Invalid Commands!\r\nThese commands don't exist: " + string.Join(", ", invalidArgs));
            }
                
        }
    }
}
