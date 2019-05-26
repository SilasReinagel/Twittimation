using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Twittimation.Http
{
    public class UrlEncodedData : IHttpKeyValuePairs
    {
        private Dictionary<string, string> _data;

        public string ContentType => "application/x-www-form-urlencoded";

        public UrlEncodedData()
        {
            _data = new Dictionary<string, string>();
        }

        public UrlEncodedData(Dictionary<string, string> data)
        {
            _data = data;
        }

        public string GetContent()
        {
            return string.Join("&",
                _data.Select(kvp => Uri.EscapeDataString(kvp.Key).Replace("%20", "+") + "=" + Uri.EscapeDataString(kvp.Value).Replace("%20", "+")));
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
