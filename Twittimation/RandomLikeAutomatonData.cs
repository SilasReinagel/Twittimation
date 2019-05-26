using System.Collections.Generic;

namespace Twittimation
{
    public sealed class RandomLikeAutomatonData
    {
        public bool IsEnabled { get; set; } = false;
        public int PercentageLikeChance { get; set; } = 10;
        public Dictionary<string, int> LikesGivenPerFollowee { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> MaxLikesPerFollowee { get; set; } = new Dictionary<string, int>();
        public long LastUpdatedTimestamp { get; set; } = 0;
    }
}
