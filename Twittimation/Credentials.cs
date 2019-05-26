namespace Twittimation
{
    public sealed class Credentials
    {
        public bool AreValid => !string.Equals("N/A", ConsumerKey);
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

        public static Credentials Invalid => new Credentials("N/A", "N/A", "N/A", "N/A");
    }
}
