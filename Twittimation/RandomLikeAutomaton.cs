using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Twittimation.Commands;
using Twittimation.Http;
using Twittimation.IO;
using Twittimation.Twitter;

namespace Twittimation
{
    public class RandomLikeAutomaton : IAutomaton
    {
        private readonly IStored<RandomLikeAutomatonData> _data;
        private readonly IStored<Tasks> _tasks;
        private readonly Like _like;
        private readonly IStored<Credentials> _credentials;
        private int _updatesToSkip = 0;
        private const int SecondsPerHour = 60 * 60;

        public RandomLikeAutomaton(IStored<RandomLikeAutomatonData> data, IStored<Tasks> tasks, Like like, IStored<Credentials> credentials)
        {
            _data = data;
            _tasks = tasks;
            _like = like;
            _credentials = credentials;
        }

        public void Update()
        {
            if(_updatesToSkip == 0)
            {
                _updatesToSkip = 11;
                var credentials = _credentials.Get();
                if (credentials.AreValid)
                {
                    var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    var random = new Random();
                    try
                    {
                        var oauthClient = new OAuthClient("https://api.twitter.com/1.1/",
                            credentials.ConsumerKey, credentials.ConsumerKeySecret, credentials.AccessToken, credentials.AccessTokenSecret);
                        var followers = JObject.Parse(oauthClient.SendRequest("GET", "friends/ids.json", new JsonData()).Result);
                        if (followers.ContainsKey("errors"))
                        {
                            Console.Error.WriteLine("Failed to get followers!");
                            return;
                        }
                        var tasks = _tasks.Get();
                        _data.Update(d => {
                            if (d.LastUpdatedTimestamp < now - now % (SecondsPerHour * 24))
                                d.LikesGivenPerFollowee = d.LikesGivenPerFollowee.ToDictionary(kvp => kvp.Key, kvp => 0);
                            foreach(var id in followers.GetValue("ids").ToObject<long[]>())
                            {
                                var tweets = JsonConvert.DeserializeObject<List<TwitterTweet>>(
                                    oauthClient.SendRequest("GET", "statuses/user_timeline.json",
                                        new Dictionary<string, string>() {
                                            { "user_id", id.ToString() },
                                            { "count", "50" },
                                            { "include_rts", "false" },
                                            { "trim_user", "true" },
                                            { "exclude_replies", "true" }
                                        },
                                        new JsonData())
                                    .Result);
                                if (d.LikesGivenPerFollowee.ContainsKey(id))
                                    tweets = tweets.Where(t => t.CreatedAt.ToUnixTimeSeconds() > d.LastUpdatedTimestamp).ToList();
                                else
                                    d.LikesGivenPerFollowee.Add(id, 0);
                                tweets = tweets.Where(t => t.CreatedAt.ToUnixTimeSeconds() > (now - SecondsPerHour * 24 * 30)).ToList();
                                var likesRemaining = d.MaxLikesPerFolloweePerDay - d.LikesGivenPerFollowee[id];
                                for(var i = 0; i < tweets.Count; i++)
                                    if(d.PercentageLikeChance > random.Next(1))
                                    {
                                        if (likesRemaining-- == 0)
                                            break;
                                        var taskId = 0;
                                        while (taskId < tasks.Count && taskId == tasks[taskId].Id)
                                            taskId++;
                                        var task = new ScheduledTask(taskId,
                                            new ScheduledOperation(DateTimeOffset.FromUnixTimeSeconds(now + random.Next(SecondsPerHour * 2)),
                                            _like.Name,
                                            tweets[i].Id.ToString()));
                                        tasks.Add(task);
                                    }
                                d.LikesGivenPerFollowee[id] = d.MaxLikesPerFolloweePerDay - likesRemaining;
                            }
                            d.LastUpdatedTimestamp = now;
                            return d;
                        });
                        _tasks.Update(t => tasks);
                    }
                    catch (Exception x)
                    {
                        Console.Error.WriteLine("Network error!");
                    }
                }
            }
            else
                _updatesToSkip--;
        }
    }
}
