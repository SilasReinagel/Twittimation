using System;
using System.Collections.Generic;

namespace Twittimation.Commands
{
    public class Help : Command
    {
        public override string Name { get; } = "Help";
        public override List<string> RequiredArgs { get; } = new List<string>();
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = "Command";
        public override string HelpInfo { get; } = "If no commands were specified this writes the help section of all commands, otherwise this writes the extended help section for those commands.";
        public override string ExtendedHelp => HelpInfo;

        private Dictionary<string, Command> _commands;

        public Help(Dictionary<string, Command> commands)
        {
            _commands = commands;
        }
        
        public override void Go(string[] args)
        {
            if (args.Length == 0)
                foreach(var command in _commands)
                {
                    Console.WriteLine("\t" + command.Value.Name);
                    Console.WriteLine(command.Value.CreateSyntaxString());
                    Console.WriteLine(command.Value.HelpInfo);
                }
            else
            {
                var invalidArgs = new List<string>();
                foreach (var arg in args)
                {
                    if (_commands.ContainsKey(arg.ToUpper()))
                    {
                        var command = _commands[arg.ToUpper()];
                        Console.WriteLine("\t" + command.Name);
                        Console.WriteLine(command.CreateSyntaxString());
                        Console.WriteLine(command.ExtendedHelp);
                    }
                    else
                    {
                        invalidArgs.Add(arg);
                    }
                }
                if (invalidArgs.Count > 0)
                    throw new ArgumentException("Invalid Commands!\r\nThese commands don't exist: " + string.Join(", ", invalidArgs));
            }
                
        }
    }
}
