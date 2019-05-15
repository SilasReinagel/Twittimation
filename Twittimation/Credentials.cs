using System;
using System.Collections.Generic;
using System.Text;

namespace Twittimation
{
    public class Credentials
    {
        public string ConsumerKey { get; set; }
        public string ConsumerKeySecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }

        public Credentials(string consumerKey, string consumerKeySecret, string accessToken, string accessTokenSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerKeySecret = consumerKeySecret;
            AccessToken = accessToken;
            AccessTokenSecret = accessTokenSecret;
        }
    }
}
