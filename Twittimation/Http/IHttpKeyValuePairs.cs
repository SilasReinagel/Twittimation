using System.Collections.Generic;

namespace Twittimation.Http
{
    public interface IHttpKeyValuePairs : IEnumerable<KeyValuePair<string, string>>
    {
        string ContentType { get; }
        string GetContent();
    }
}