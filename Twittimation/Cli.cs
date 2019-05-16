using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twittimation.Commands;

namespace Twittimation
{
    public class Cli
    {
        public Dictionary<string, Command> Commands { get; private set; }
        private string _commandNotFoundMessage;

        public Cli(string commandNotFoundMessage, params Command[] commands)
        {
            _commandNotFoundMessage = commandNotFoundMessage;
            Commands = new Dictionary<string, Command>(StringComparer.CurrentCultureIgnoreCase);
            AddCommands(commands);
        }

        public void AddCommands(params Command[] commands)
        {
            foreach (var command in commands)
                AddCommand(command);
        }

        public void AddCommand(Command command)
        {
            Commands.Add(command.Name.ToUpper(), command);
        }

        public void Execute(string input, bool exit = false)
        {
            Execute(ReadCommandLineArgs(input), exit);
        }

        public void Execute(string[] args, bool exit = false)
        {
            try
            {
                if (args.Length > 0 && Commands.ContainsKey(args.First()))
                    Commands[args.First()].Go(args.SubArray(1, args.Length - 1));
                else
                    Console.Error.WriteLine(_commandNotFoundMessage);
            }
            catch (ArgumentException x)
            {
                Console.Error.WriteLine(x.Message);
                if (exit)
                    Environment.Exit(1);
            }
            if (exit)
                Environment.Exit(0);
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
