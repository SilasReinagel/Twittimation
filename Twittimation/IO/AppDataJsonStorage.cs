using System;
using System.IO;
using Newtonsoft.Json;

namespace Twittimation.IO
{
    public sealed class AppDataJsonStorage : IStorage
    {
        private readonly string _appStorageFolder;

        public AppDataJsonStorage(string appFolderName)
        {
            _appStorageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appFolderName);
        }

        public bool Exists(string saveName) => File.Exists(GetSavePath(saveName));
        public T Get<T>(string key) => JsonConvert.DeserializeObject<T>(File.ReadAllText(GetSavePath(key)));
        public void Remove(string saveName) => File.Delete(GetSavePath(saveName));
        public void Put<T>(string key, T value)
        {
            if (!Directory.Exists(_appStorageFolder))
                Directory.CreateDirectory(_appStorageFolder);
            File.WriteAllText(GetSavePath(key), JsonConvert.SerializeObject(value));
        }

        private string GetSavePath(string saveName) => Path.Combine(_appStorageFolder, saveName + ".json");
    }
}
