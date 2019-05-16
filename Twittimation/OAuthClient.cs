using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> SendRequest(string relativeUrl, IEnumerable<KeyValuePair<string, string>> data)
        {
            var fullUrl = _apiUrl + relativeUrl;
            var dataAsDictionary = data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            AddOauthData(fullUrl, dataAsDictionary);
            var request = new HttpRequest(fullUrl, "POST", string.Join("&", data.Where(kvp => !kvp.Key.StartsWith("oauth_"))
                    .Select(kvp => Uri.EscapeDataString(kvp.Key).Replace("%20", "+") + "=" + Uri.EscapeDataString(kvp.Value).Replace("%20", "+"))),
                new KeyValuePair<string, string>("Authorization", GenerateOAuthHeader(dataAsDictionary)),
                new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded"));
            await request.Go();
            return request.ResponseAsString;
        }

        private void AddOauthData(string url, Dictionary<string, string> data)
        {
            data.Add("oauth_consumer_key", _consumerKey);
            data.Add("oauth_token", _accessToken);
            data.Add("oauth_signature_method", "HMAC-SHA1");
            data.Add("oauth_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            data.Add("oauth_nonce", Guid.NewGuid().ToString());
            data.Add("oauth_version", "1.0");
            data.Add("oauth_signature", GenerateSignature(url, data));
        }

        private string GenerateSignature(string url, Dictionary<string, string> data)
        {
            var signatureData = "POST&" + Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(string.Join("&",
                data.Select(kvp => Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value)).OrderBy(s => s)));
            return Convert.ToBase64String(_encryptor.ComputeHash(new ASCIIEncoding().GetBytes(signatureData)));
        }

        private string GenerateOAuthHeader(Dictionary<string, string> data)
        {
            return "OAuth " + string.Join(",", data
                .Where(kvp => kvp.Key.StartsWith("oauth_"))
                .Select(kvp => Uri.EscapeDataString(kvp.Key) + "=\"" + Uri.EscapeDataString(kvp.Value) +"\""));
        }
    }
}
