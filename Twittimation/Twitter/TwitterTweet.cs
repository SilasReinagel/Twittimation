using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Twittimation.Twitter
{
    public class TwitterTweet
    {
        [JsonIgnore]
        public DateTimeOffset CreatedAt
            => DateTimeOffset.ParseExact(_createdAt, "ddd MMM dd HH:mm:ss zzz yyyy", new System.Globalization.CultureInfo("en-US"));
        [JsonProperty("created_at")]
        private string _createdAt;
        public long Id { get; set; }
    }
}
