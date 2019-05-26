using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Carvana;
using Twittimation.Http;
using Twittimation.IO;

namespace Twittimation.Twitter
{
    public sealed class HttpTwitterClient : ITwitterGateway
    {
        private readonly IStored<Credentials> _credentials;

        public HttpTwitterClient(IStored<Credentials> credentials)
        {
            _credentials = credentials;
        }

        public async Task<Result<List<TweetData>>> GetNewTweets(long afterUnixSeconds, string userId)
            => await SendRequest(
                    "GET",
                    "statuses/user_timeline.json",
                    new Dictionary<string, string>()
                    {
                        {"user_id", userId},
                        {"count", "50"},
                        {"include_rts", "false"},
                        {"trim_user", "true"},
                        {"exclude_replies", "true"}
                    },
                    new JsonData())
                .Then(x => x
                    .ToObject<List<TweetData>>()
                    .Where(t => t.CreatedAt.ToUnixTimeSeconds() > afterUnixSeconds)
                    .ToList());

        public async Task<Result> Like(string tweetId)
            => await SendRequest("POST", "favorites/create.json", new Dictionary<string, string> {{"id", tweetId}}, new JsonData());

        public async Task<Result<string>> GetUserId(string username)
            => await SendRequest("GET", "users/lookup.json", new Dictionary<string, string> { { "screen_name", username } }, new JsonData())
                .Then(x => x.GetValue("id").ToObject<long>().ToString());

        public async Task<Result> Tweet(string tweetContent)
            => await SendRequest("POST", "statuses/update.json", new Dictionary<string, string>(), 
                new UrlEncodedData(new Dictionary<string, string> { { "status", tweetContent }, { "trim_user", "1" } }));

        public async Task<Result<List<string>>> GetFriendIds()
            => await SendRequest("GET", "friends/ids.json", new JsonData())
                .Then(x => x.GetValue("ids").ToObject<long[]>().Select(id => id.ToString()).ToList());

        private async Task<Result<JObject>> SendRequest(string method, string relativeUrl, IHttpKeyValuePairs data)
            => await SendRequest(method, relativeUrl, new Dictionary<string, string>(), data);

        private async Task<Result<JObject>> SendRequest(
            string method, string relativeUrl, Dictionary<string, string> query, IHttpKeyValuePairs data)
        {
            var credentials = _credentials.Get();
            if (!credentials.AreValid)
                return Result<JObject>.Errored(ResultStatus.ClientError, "Credentials have not been setup. Use 'SaveCredentials' to initialize.");

            var client = new OAuthClient("https://api.twitter.com/1.1/", credentials);
            
            var result = JObject.Parse(await client.SendRequest(method, relativeUrl, query, data));
            return result.ContainsKey("errors")
                ? Result<JObject>.Errored(ResultStatus.DependencyFailure, ToErrorMessage(result))
                : result; 
        }

        private string ToErrorMessage(JObject response)
            => string.Join(Environment.NewLine, response.GetValue("errors").ToObject<TwitterError[]>().Select(e => e.Message));
    }
}
