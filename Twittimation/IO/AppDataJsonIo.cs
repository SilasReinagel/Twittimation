using System;
using System.IO;
using Newtonsoft.Json;

namespace Twittimation.IO
{
    public sealed class AppDataJsonIo
    {
        private readonly string _appStorageFolder;

        public AppDataJsonIo(string appFolderName)
        {
            _appStorageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appFolderName);
        }

        public T Load<T>(string saveName)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(GetSavePath(saveName)));
        }

        public T LoadOrDefault<T>(string saveName, Func<T> createDefault)
        {
            try
            {
                return HasSave(saveName) ? Load<T>(saveName) : createDefault();
            }
            catch (Exception x)
            {
                return createDefault();
            }
        }

        public void Save(string saveName, object data)
        {
            if (!Directory.Exists(_appStorageFolder))
                Directory.CreateDirectory(_appStorageFolder);
            File.WriteAllText(GetSavePath(saveName), JsonConvert.SerializeObject(data));
        }

        public bool HasSave(string saveName)
        {
            return File.Exists(GetSavePath(saveName));
        }

        public void Delete(string saveName)
        {
            File.Delete(GetSavePath(saveName));
        }

        private string GetSavePath(string saveName)
        {
            return Path.Combine(_appStorageFolder, saveName + ".json");
        }
    }
}