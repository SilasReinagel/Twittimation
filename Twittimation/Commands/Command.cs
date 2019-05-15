using System;
using System.Collections.Generic;
using System.Linq;

namespace Twittimation.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract List<string> RequiredArgs { get; }
        public abstract List<string> OptionalArgs { get; }
        public abstract Optional<string> OptionalRepeatedArg { get; }
        public abstract string HelpInfo { get; }
        public abstract string ExtendedHelp { get; }

        public abstract void Go(string[] args);

        public string CreateSyntaxString()
        {
            return $"Syntax: {Name}{string.Concat(RequiredArgs.Select(a => " <" + a + ">"))}{string.Concat(OptionalArgs.Select(a => " [<" + a + ">]"))}{(OptionalRepeatedArg.HasValue ? " [<" + OptionalRepeatedArg.Value + ">]...": "")}";
        }

        protected void ValidateArgCount(string[] args)
        {
            if (args.Length < RequiredArgs.Count)
                ThrowInsufficentArgs();
            else if (!OptionalRepeatedArg.HasValue && args.Length > RequiredArgs.Count + OptionalArgs.Count)
                ThrowTooManyArgs();
        }

        protected void ThrowInsufficentArgs()
        {
            throw new ArgumentException("Too few args!\r\n" + CreateSyntaxString());
        }

        protected void ThrowTooManyArgs()
        {
            throw new ArgumentException("Too many args!\r\n" + CreateSyntaxString());
        }
    }
}
