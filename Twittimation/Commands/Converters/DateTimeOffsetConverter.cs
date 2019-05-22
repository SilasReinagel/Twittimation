using System;
using System.Collections.Generic;
using System.Text;

namespace Twittimation.Commands.Converters
{
    public static class DateTimeOffsetConverter
    {
        public static DateTimeOffset Convert(string arg)
        {
            if (!DateTimeOffset.TryParse(arg, out DateTimeOffset time))
                throw new UserErrorException("Invalid time format! Time can be formatted as \"Year/Month/Day Hour:Minute:Second\"");
            if (time < DateTimeOffset.Now)
                throw new UserErrorException("Time is in the past!");
            return time;
        }
    }
}
