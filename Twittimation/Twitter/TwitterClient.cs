using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Twittimation.Http;

namespace Twittimation.Twitter
{
    public class TwitterClient
    {
        private OAuthClient _oAuthClient;

        public TwitterClient(Credentials credentials)
        {
            _oAuthClient = new OAuthClient("https://api.twitter.com/1.1/",
                credentials.ConsumerKey, credentials.ConsumerKeySecret, credentials.AccessToken, credentials.AccessTokenSecret);
        }

        public async Task<JObject> SendRequest(string method, string relativeUrl, IHttpKeyValuePairs data)
        {
            return await SendRequest(method, relativeUrl, new Dictionary<string, string>(), data);
        }

        public async Task<JObject> SendRequest(
            string method, string relativeUrl, Dictionary<string, string> query, IHttpKeyValuePairs data)
        {
            var result = JObject.Parse(await _oAuthClient.SendRequest(method, relativeUrl, query, data));
            if (result.ContainsKey("errors"))
                throw new TwitterException(result.GetValue("errors").ToObject<TwitterError[]>());
            return result;
        }
    }
}
