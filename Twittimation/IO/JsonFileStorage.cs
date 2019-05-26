using System;
using System.IO;
using Newtonsoft.Json;

namespace Twittimation.IO
{
    public sealed class JsonFileStorage : IStorage
    {
        private readonly string _dataFolderPath;

        public JsonFileStorage(string dataFolderPath)
        {
            _dataFolderPath = dataFolderPath;
        }

        public bool Exists(string saveName) => File.Exists(GetSavePath(saveName));
        public T Get<T>(string key) => JsonConvert.DeserializeObject<T>(File.ReadAllText(GetSavePath(key)));
        public void Remove(string saveName) => File.Delete(GetSavePath(saveName));
        public void Put<T>(string key, T value)
        {
            if (!Directory.Exists(_dataFolderPath))
                Directory.CreateDirectory(_dataFolderPath);
            File.WriteAllText(GetSavePath(key), JsonConvert.SerializeObject(value, Formatting.Indented));
        }

        private string GetSavePath(string saveName) => Path.Combine(_dataFolderPath, saveName + ".json");
        
        public static IStorage AppData(string appName) => new JsonFileStorage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName));
    }
}
