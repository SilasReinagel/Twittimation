﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Twittimation.Commands
{
    public abstract class Command
    {
        public string Name => GetType().Name;
        public abstract List<string> RequiredArgs { get; }
        public abstract List<string> OptionalArgs { get; }
        public abstract Optional<string> OptionalRepeatedArg { get; }
        public abstract string HelpInfo { get; }
        public abstract string ExtendedHelp { get; }

        protected abstract void Go(string[] args);
        public void Execute(string[] args)
        {
            ValidateArgCount(args);
            Go(args);
        }

        public string CreateSyntaxString()
        {
            return $"{Name}{string.Concat(RequiredArgs.Select(a => " <" + a + ">"))}{string.Concat(OptionalArgs.Select(a => " [<" + a + ">]"))}{(OptionalRepeatedArg.HasValue ? " [<" + OptionalRepeatedArg.Value + ">]...": "")}";
        }

        protected void ValidateArgCount(string[] args)
        {
            if (args.Length < RequiredArgs.Count)
                ThrowInsufficientArgs();
            else if (!OptionalRepeatedArg.HasValue && args.Length > RequiredArgs.Count + OptionalArgs.Count)
                ThrowTooManyArgs();
        }

        protected void ThrowInsufficientArgs()
        {
            throw new UserErrorException("Too few args!\r\nSyntax: " + CreateSyntaxString());
        }

        protected void ThrowTooManyArgs()
        {
            throw new UserErrorException("Too many args!\r\nSyntax: " + CreateSyntaxString());
        }

        protected void RethrowExceptionsAsUserError(Action action, params Type[] exceptionTypes)
        {
            RethrowExceptionsAsUserError((Delegate)action, exceptionTypes);
        }

        protected T RethrowExceptionsAsUserError<T>(Func<T> action, params Type[] exceptionTypes)
        {
            return (T)RethrowExceptionsAsUserError((Delegate)action, exceptionTypes);
        }

        private object RethrowExceptionsAsUserError(Delegate action, Type[] exceptionTypes)
        {
            try
            {
                return action.DynamicInvoke();
            }
            catch (UserErrorException x)
            {
                throw x;
            }
            catch (Exception x)
            {
                var type = x.GetType();
                if (exceptionTypes.Any(t => type.IsAssignableFrom(t)))
                    throw new UserErrorException("Error\r\n" + x.GetType().ToString() + "\r\n" + x.Message);
                throw x;
            }
        }
    }
}
