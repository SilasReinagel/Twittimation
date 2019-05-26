using System;

namespace Twittimation.Commands.Converters
{
    public static class DateTimeOffsetConverter
    {
        public static DateTimeOffset ParseFutureTime(string arg)
        {
            if (!DateTimeOffset.TryParse(arg, out DateTimeOffset time))
                throw new UserErrorException("Invalid time format! Time can be formatted as \"Year/Month/Day Hour:Minute:Second\"");
            if (time <= DateTimeOffset.Now)
                throw new UserErrorException("Time is not in the future!");
            return time;
        }
    }
}
