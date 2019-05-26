using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Carvana;
using Twittimation.Commands;
using Twittimation.Http;
using Twittimation.IO;
using Twittimation.Twitter;

namespace Twittimation
{
    public sealed class RandomLikeAutomaton : IAutomaton
    {
        private readonly IStored<RandomLikeAutomatonData> _data;
        private readonly IStored<Tasks> _tasks;
        private readonly Like _like;
        private readonly ITwitterGateway _twitter;
        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        private TimeSpan _timeTilNextUpdate = TimeSpan.Zero;
        private const int SecondsPerHour = 60 * 60;

        public RandomLikeAutomaton(IStored<RandomLikeAutomatonData> data, IStored<Tasks> tasks, Like like, ITwitterGateway twitter)
        {
            _data = data;
            _tasks = tasks;
            _like = like;
            _twitter = twitter;
        }

        public void Update(TimeSpan delta)
        {
            if (!_data.Get().IsEnabled)
                return; 
            
            _timeTilNextUpdate -= delta;
            if(_timeTilNextUpdate <= TimeSpan.Zero)
            {
                Update();
                _timeTilNextUpdate = TimeSpan.FromSeconds(60);
            }
        }

        private void Update()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            try
            {
                var followeesResp = _twitter.GetFriendIds().GetAwaiter().GetResult();
                followeesResp.OnSuccess(ids =>
                {
                    var updatedTasks = _tasks.Get();
                    _data.Update(d =>
                    {
                        if (d.LastUpdatedTimestamp < now - now % (SecondsPerHour * 24))
                            d.LikesGivenPerFollowee = d.LikesGivenPerFollowee.ToDictionary(kvp => kvp.Key, kvp => 0);
                        ScheduleLikesForFollowees(now, ids, d, updatedTasks).GetAwaiter().GetResult();
                        d.LastUpdatedTimestamp = now;
                        return d;
                    });
                    _tasks.Update(t => updatedTasks);
                });
            }
            catch (TwitterException x)
            {
                Console.Error.WriteLine($"Twitter error! {x}");
            }
            catch (Exception x)
            {
                Console.Error.WriteLine($"Network error! {x}");
            }
        }

        private async Task<Result> ScheduleLikesForFollowees(long now, IEnumerable<string> userIds, RandomLikeAutomatonData data, Tasks tasks)
        {
            foreach (var id in userIds)
            {
                var tweetsResult = await _twitter.GetNewTweets(data.MaxLikesPerFollowee.ContainsKey(id) ? data.LastUpdatedTimestamp : now - SecondsPerHour * 24, id.ToString());
                if (!tweetsResult.Succeeded())
                    return tweetsResult;
                
                if (!data.MaxLikesPerFollowee.ContainsKey(id))
                {
                    data.MaxLikesPerFollowee.Add(id, 0);
                    data.LikesGivenPerFollowee.Add(id, 0);
                }
                ScheduleLikesForFollowee(now, data, tasks, id, tweetsResult.Content);
            }
            return Result.Success();
        }

        private void ScheduleLikesForFollowee(long now, RandomLikeAutomatonData data, Tasks tasks, string userId, List<TweetData> tweets)
        {
            for (var i = 0; i < tweets.Count; i++)
                if (data.PercentageLikeChance > _random.Next(1) && data.LikesGivenPerFollowee[userId] < data.MaxLikesPerFollowee[userId])
                {
                    var operation = new ScheduledOperation(DateTimeOffset.FromUnixTimeSeconds(now + _random.Next(SecondsPerHour * 2)),
                        _like.Name,
                        tweets[i].Id.ToString());
                    tasks.Add(CreateTaskWithUniqueId(tasks, operation));
                    data.LikesGivenPerFollowee[userId]++;
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
    }
}
