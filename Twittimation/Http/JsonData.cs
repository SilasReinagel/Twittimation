using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Twittimation.Http
{
    public class JsonData : IHttpKeyValuePairs
    {
        private Dictionary<string, object> _data;

        public string ContentType => "application/json";

        public JsonData()
        {
            _data = new Dictionary<string, object>();
        }

        public JsonData(Dictionary<string, object> data)
        {
            _data = data;
        }

        public string GetContent()
        {
            return JsonConvert.SerializeObject(_data);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _data.Select(kvp => new KeyValuePair<string, string>(kvp.Key, JsonConvert.SerializeObject(kvp.Value))).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
