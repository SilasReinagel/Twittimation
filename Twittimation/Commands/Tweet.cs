using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Twittimation.IO;

namespace Twittimation.Commands
{
    public class Tweet : Command
    {
        public override string Name { get; } = "Tweet";
        public override List<string> RequiredArgs { get; } = new List<string>() { "Text" };
        public override List<string> OptionalArgs { get; } = new List<string>();
        public override Optional<string> OptionalRepeatedArg { get; } = new Optional<string>();
        public override string HelpInfo { get; } = "Posts a tweet with the specified text.";
        public override string ExtendedHelp { get; } = "Posts a tweet with the specified text using the saved credentials.";

        private AppDataJsonIo _io;

        public Tweet(AppDataJsonIo io)
        {
            _io = io;
        }

        public override void Go(string[] args)
        {
            ValidateArgCount(args);
            if (!_io.HasSave("Credentials"))
                Console.Error.WriteLine("No credentials were provided for this request");
            try
            {
                var credentials = _io.Load<Credentials>(Program.TwitterCredentialsFileName);
                var client = new OAuthClient("https://api.twitter.com/1.1/",
                    credentials.ConsumerKey, credentials.ConsumerKeySecret, credentials.AccessToken, credentials.AccessTokenSecret);
                var result = JObject.Parse(client.SendRequest("statuses/update.json",
                    new Dictionary<string, string>() { { "status", args[0] }, { "trim_user", "1" }}).Result);
                if (result.ContainsKey("errors"))
                {
                    var error = result.GetValue("errors").ToObject<TwitterError[]>();
                    if (error.Length > 0)
                    {
                        Console.Error.WriteLine("Twitter error: ");
                        Console.Error.WriteLine("code: " + error[0].Code.ToString());
                        Console.Error.WriteLine("message: " + error[0].Message);
                    }
                }
                else
                    Console.WriteLine("Success");
            }
            catch (IOException)
            {
                Console.Error.WriteLine("Failed to read credentials file!");
            }
            catch (JsonException)
            {
                Console.Error.WriteLine("Credentials were misformatted!");
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Network error!");
            }
        }

        public string CreateCommandString(string text)
        {
            return Name + " \"" + text + "\"";
        }
    }
}
