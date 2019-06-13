using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly ILog _log;

        public Help(Dictionary<string, Command> commands, ILog log)
        {
            _commands = commands;
            _log = log;
        }
        
        protected override void Go(string[] args)
        {
            if (args.Length == 0)
            {
                _log.Info($"usage: twittimation <command> [<args>]{Environment.NewLine}");
                _log.Info($"All Commands:");
                foreach (var command in _commands)
                    _log.Info("\t" + command.Value.CreateSyntaxString());
            }
            else
            {
                var invalidArgs = new List<string>();
                foreach (var arg in args)
                {
                    if (_commands.ContainsKey(arg.ToUpper()))
                    {
                        var command = _commands[arg.ToUpper()];
                        _log.Info("\t" + command.CreateSyntaxString());
                        _log.Info("\t" + string.Join("\t\r\n", command.ExtendedHelp.Split("\r\n")));
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
