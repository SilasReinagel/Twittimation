using System.Collections.Generic;
using System.Threading.Tasks;
using Carvana;

namespace Twittimation.Twitter
{
    public sealed class TwitterStub : ITwitterGateway
    {
        public Task<Result<List<TweetData>>> GetNewTweets(long sinceUnixSeconds, string userId) => Task.FromResult(Result.Success(new List<TweetData>()));
        public Task<Result> Like(string tweetId) => Task.FromResult(Result.Success());
        public Task<Result<string>> GetUserId(string username) => Task.FromResult(Result.Success("UserId")); 
        public Task<Result> Tweet(string tweetContent) => Task.FromResult(Result.Success());
        public Task<Result<List<string>>> GetFriendIds() => Task.FromResult(Result.Success<List<string>>(new List<string> { "UserId" }));
    }
}
