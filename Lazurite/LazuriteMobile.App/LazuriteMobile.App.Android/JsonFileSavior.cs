using Lazurite.Data;
using Newtonsoft.Json;
using System;
using System.IO;

namespace LazuriteMobile.App.Droid
{
    public class JsonFileSavior : SaviorBase
    {
        private string _baseDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private string _ext = ".json";

        public override void Clear(string key)
        {
            var path = GetPath(key);
            if (File.Exists(path))
                File.Delete(path);
        }

        public override T Get<T>(string key)
        {
            var path = GetPath(key);
            var data = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        public override bool Has(string key)
        {
            var path = GetPath(key);
            var result = File.Exists(path);
            return result;
        }

        public override void Set<T>(string key, T obj)
        {
            var path = GetPath(key);
            var data = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            File.WriteAllText(path, data);
        }

        private string GetPath(string itemName)
        {
            return Path.Combine(_baseDir, itemName + _ext);
        }
    }
}