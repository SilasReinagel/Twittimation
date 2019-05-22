using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Twittimation.Twitter
{
    public class TwitterException : Exception
    {
        public TwitterError[] Errors { get; }

        public TwitterException(params TwitterError[] errors)
        {
            if (errors.Length == 0)
                throw new ArgumentOutOfRangeException("Errors is empty!");
            Errors = errors;
        }

        public override string Message => String.Join("\r\n", Errors.Select(e => "Code: " + Errors[0].Code + "\r\nMessage: " + Errors[0].Message));
    }
}
