using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Twittimation
{
    public class UserErrorException : Exception
    {
        public UserErrorException(string message) : base(message)
        {
        }

        public UserErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
