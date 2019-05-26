using System.Collections.Generic;

namespace Twittimation.IO
{
    public sealed class InMemoryStorage : IStorage
    {
        private readonly Dictionary<string, object> _objects = new Dictionary<string, object>();

        public bool Exists(string key) => _objects.ContainsKey(key);
        public void Put<T>(string key, T value) => _objects[key] = value;
        public void Remove(string key) => _objects.Remove(key);
        public T Get<T>(string key) => (T) _objects[key];
    }
}
