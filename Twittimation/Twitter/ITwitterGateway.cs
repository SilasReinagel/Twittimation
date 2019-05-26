using System.Collections.Generic;
using System.Threading.Tasks;
using Carvana;

namespace Twittimation.Twitter
{
    public interface ITwitterGateway
    {
        Task<Result<List<TweetData>>> GetNewTweets(long sinceUnixSeconds, string userId);
        Task<Result> Like(string tweetId);
        Task<Result<string>> GetUserId(string username);
        Task<Result> Tweet(string tweetContent);
        Task<Result<List<string>>> GetFriendIds();
    }
}
