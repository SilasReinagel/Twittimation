using System;

namespace Twittimation.IO
{
    public sealed class KeyStored<T> : IStored<T>
    {
        private readonly IStorage _storage;
        private readonly string _key;
        private readonly Func<T> _createDefault;

        public KeyStored(IStorage storage, string key, Func<T> createDefault)
        {
            _storage = storage;
            _key = key;
            _createDefault = createDefault;
        }

        public T Get() => _storage.GetOrDefault(_key, _createDefault);

        public void Update(Func<T, T> update) => _storage.Put(_key, update(Get()));
    }
}
