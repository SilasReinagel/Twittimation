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
        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        private TimeSpan _timeTilNextUpdate = TimeSpan.Zero;
        private const int SecondsPerHour = 60 * 60;

        public RandomLikeAutomaton(IStored<RandomLikeAutomatonData> data, IStored<Tasks> tasks, Like like, IStored<Credentials> credentials)
        {
            _data = data;
            _tasks = tasks;
            _like = like;
            _credentials = credentials;
        }

        public void Update(TimeSpan delta)
        {
            _timeTilNextUpdate -= delta;
            if(_timeTilNextUpdate <= TimeSpan.Zero)
            {
                Update();
                _timeTilNextUpdate = TimeSpan.FromSeconds(60);
            }
        }

        private void Update()
        {
            var credentials = _credentials.Get();
            if (credentials.AreValid)
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                try
                {
                    var client = new TwitterClient(credentials);
                    var followees = client.SendRequest("GET", "friends/ids.json", new JsonData()).GetAwaiter().GetResult();
                    var tasks = _tasks.Get();
                    _data.Update(d =>
                    {
                        if (d.LastUpdatedTimestamp < now - now % (SecondsPerHour * 24))
                            d.LikesGivenPerFollowee = d.LikesGivenPerFollowee.ToDictionary(kvp => kvp.Key, kvp => 0);
                        ScheduleLikesForFollowees(now, client, followees.GetValue("ids").ToObject<long[]>(), d, tasks);
                        d.LastUpdatedTimestamp = now;
                        return d;
                    });
                    _tasks.Update(t => tasks);
                }
                catch (TwitterException x)
                {
                    Console.Error.WriteLine("Twitter error!");
                }
                catch (Exception x)
                {
                    Console.Error.WriteLine("Network error!");
                }
            }
        }

        private void ScheduleLikesForFollowees(long now, TwitterClient client, long[] followees, RandomLikeAutomatonData data, Tasks tasks)
        {
            foreach (var id in followees)
            {
                List<TwitterTweet> tweets = GetNewTweets(data.MaxLikesPerFollowee.ContainsKey(id) ? data.LastUpdatedTimestamp : now - SecondsPerHour * 24,
                    id, client);
                if (!data.MaxLikesPerFollowee.ContainsKey(id))
                {
                    data.MaxLikesPerFollowee.Add(id, 0);
                    data.LikesGivenPerFollowee.Add(id, 0);
                }
                ScheduleLikesForFollowee(now, data, tasks, id, tweets);
            }
        }

        private void ScheduleLikesForFollowee(long now, RandomLikeAutomatonData data, Tasks tasks, long id, List<TwitterTweet> tweets)
        {
            for (var i = 0; i < tweets.Count; i++)
                if (data.PercentageLikeChance > _random.Next(1) && data.LikesGivenPerFollowee[id] < data.MaxLikesPerFollowee[id])
                {
                    var operation = new ScheduledOperation(DateTimeOffset.FromUnixTimeSeconds(now + _random.Next(SecondsPerHour * 2)),
                        _like.Name,
                        tweets[i].Id.ToString());
                    tasks.Add(CreateTaskWithUniqueId(tasks, operation));
                    data.LikesGivenPerFollowee[id]++;
                }
        }

        private static ScheduledTask CreateTaskWithUniqueId(Tasks tasks, ScheduledOperation operation)
        {
            var taskId = 0;
            while (taskId < tasks.Count && taskId == tasks[taskId].Id)
                taskId++;
            var task = new ScheduledTask(taskId, operation);
            return task;
        }

        private static List<TwitterTweet> GetNewTweets(long since, long id, TwitterClient client)
        {
            return client.SendRequest(
                    "GET",
                    "statuses/user_timeline.json",
                    new Dictionary<string, string>() {
                        { "user_id", id.ToString() },
                        { "count", "50" },
                        { "include_rts", "false" },
                        { "trim_user", "true" },
                        { "exclude_replies", "true" }
                    },
                    new JsonData())
                .GetAwaiter()
                .GetResult()
                .ToObject<List<TwitterTweet>>()
                .Where(t => t.CreatedAt.ToUnixTimeSeconds() > since)
                .ToList();
        }
    }
}
