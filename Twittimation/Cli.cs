using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Carvana;

namespace Twittimation
{
    public class Cli
    {
        private readonly ILog _log;
        private readonly Dictionary<string, ICommand> _commands;

        public Cli(ILog log, params ICommand[] commands)
        {
            _log = log;
            _commands = new Dictionary<string, ICommand>(StringComparer.InvariantCultureIgnoreCase);
            AddCommands(commands);
        }

        public void AddCommands(params ICommand[] commands)
        {
            foreach (var command in commands)
                AddCommand(command);
        }

        public void AddCommand(ICommand command)
        {
            _commands.Add(command.Name, command);
        }

        public Result Execute(string input)
        {
            return Execute(ReadCommandLineArgs(input));
        }

        public Result Execute(string[] args)
        {
            var result = Result.Success();
            try
            {
                if (args.Length > 0 && _commands.ContainsKey(args.First()))
                {
                    _commands[args.First()].Execute(args.SubArray(1, args.Length - 1));
                }
                else
                    result = Result.InvalidRequest($"Unknown Command '{args[0]}', type 'help' to display all commands with their help sections");
            }
            catch (UserErrorException x)
            {
                result = Result.Errored(ResultStatus.InvalidRequest, x.Message);
            }
            
            return result.OnFailure(x => _log.Error(result.ErrorMessage));
        }

        private string[] ReadCommandLineArgs(string input)
        {
            List<string> args = new List<string>();
            var currentArg = new StringBuilder();
            bool escape = false;
            bool inQuote = false;
            bool hadQuote = false;
            char prevCh = '\0';
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch == '\\')
                {
                    if (escape)
                        currentArg.Append(ch);
                    escape = !escape;
                }
                else if (ch == '"' && !escape)
                {
                    inQuote = !inQuote;
                    hadQuote = true;
                    if (inQuote && prevCh == '"')
                        currentArg.Append(ch);
                }
                else if (ch == '"' && escape)
                {
                    currentArg.Append(ch);
                    escape = false;
                }
                else if (char.IsWhiteSpace(ch) && !inQuote)
                {
                    if (escape)
                    {
                        currentArg.Append('\\');
                        escape = false;
                    }
                    if (currentArg.Length > 0 || hadQuote)
                        args.Add(currentArg.ToString());
                    currentArg.Clear();
                    hadQuote = false;
                }
                else
                {
                    if (escape)
                    {
                        currentArg.Append('\\');
                        escape = false;
                    }
                    currentArg.Append(ch);
                }
                prevCh = ch;
            }
            if (currentArg.Length > 0 || hadQuote)
                args.Add(currentArg.ToString());
            return args.ToArray();
        }
    }
}
