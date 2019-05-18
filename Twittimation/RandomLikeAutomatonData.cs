using System.Collections.Generic;

namespace Twittimation
{
    public class RandomLikeAutomatonData
    {
        public int MaxLikesPerFolloweePerDay { get; set; } = 0;
        public int PercentageLikeChance { get; set; } = 10;
        public Dictionary<long, int> LikesGivenPerFollowee { get; set; } = new Dictionary<long, int>();
        public long LastUpdatedTimestamp { get; set; } = 0;
    }
}