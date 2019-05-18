using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Twittimation.Http;

namespace Twittimation
{
    public class OAuthClient
    {
        private string _apiUrl;
        private string _consumerKey;
        private string _consumerKeySecret;
        private string _accessToken;
        private string _accessTokenSecret;
        private HMACSHA1 _encryptor;
        
        public OAuthClient(string apiUrl, string consumerKey, string consumerKeySecret, string accessToken, string accessTokenSecret)
        {
            _apiUrl = apiUrl;
            _consumerKey = consumerKey;
            _consumerKeySecret = consumerKeySecret;
            _accessToken = accessToken;
            _accessTokenSecret = accessTokenSecret;
            _encryptor = new HMACSHA1(new ASCIIEncoding().GetBytes(consumerKeySecret + "&" + accessTokenSecret));
        }

        public async Task<string> SendRequest(string method, string relativeUrl, IHttpKeyValuePairs data)
        {
            return await SendRequest(method, relativeUrl, new List<KeyValuePair<string, string>>(), data);
        }

        public async Task<string> SendRequest(string method, string relativeUrl,
            IEnumerable<KeyValuePair<string, string>> query, IHttpKeyValuePairs data)
        {
            var baseUrl = _apiUrl + relativeUrl;
            HttpRequest request;
            if(method == "GET")
                request = new HttpRequest(baseUrl + (query.Count() == 0 ? "" :
                        "?" + string.Join("&", query.Select(kvp => Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value)))),
                    method,
                    new KeyValuePair<string, string>("Authorization", GenerateOAuthHeader(method, baseUrl, query, data)),
                    new KeyValuePair<string, string>("Content-Type", data.ContentType));
            else
                request = new HttpRequest(baseUrl + (query.Count() == 0 ? "" :
                        "?" + string.Join("&", query.Select(kvp => Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value)))),
                    method,
                    data.GetContent(),
                    new KeyValuePair<string, string>("Authorization", GenerateOAuthHeader(method, baseUrl, query, data)),
                    new KeyValuePair<string, string>("Content-Type", data.ContentType));
            await request.Go();
            return request.ResponseAsString;
        }

        private string GenerateSignature(string method, string url, Dictionary<string, string> data)
        {
            var signatureData = method + "&" + Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(string.Join("&",
                data.Select(kvp => Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value)).OrderBy(s => s)));
            return Convert.ToBase64String(_encryptor.ComputeHash(new ASCIIEncoding().GetBytes(signatureData)));
        }

        private string GenerateOAuthHeader(string method, string baseUrl, IEnumerable<KeyValuePair<string, string>> query, IHttpKeyValuePairs data)
        {
            var oauthData = new Dictionary<string, string>
            {
                { "oauth_consumer_key", _consumerKey },
                { "oauth_token", _accessToken },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
                { "oauth_nonce", Guid.NewGuid().ToString() },
                { "oauth_version", "1.0" }
            };
            oauthData.Add("oauth_signature",
                GenerateSignature(method, baseUrl, data.Union(oauthData).Union(query).ToDictionary(k => k.Key, v => v.Value)));
            return "OAuth " + string.Join(",", oauthData
                .Select(kvp => Uri.EscapeDataString(kvp.Key) + "=\"" + Uri.EscapeDataString(kvp.Value) + "\""));
        }
    }
}
