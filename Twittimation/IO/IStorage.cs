using System;

namespace Twittimation.IO
{
    public interface IStorage
    {
        bool Exists(string key);
        void Put<T>(string key, T value);
        void Remove(string key);
        T Get<T>(string key);
    }

    public static class StorageExtensions
    {
        public static T GetOrDefault<T>(this IStorage storage, string key, Func<T> createDefault)
        {
            try
            {
                return storage.Exists(key) ? storage.Get<T>(key) : createDefault();
            }
            catch (Exception)
            {
                return createDefault();
            }
        }
    }
}
