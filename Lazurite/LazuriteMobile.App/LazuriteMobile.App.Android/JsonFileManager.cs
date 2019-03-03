using Lazurite.Data;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace LazuriteMobile.App.Droid
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2146:TypesMustBeAtLeastAsCriticalAsBaseTypesFxCopRule")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2132:DefaultConstructorsMustHaveConsistentTransparencyFxCopRule")]
    public class JsonFileManager : DataManagerBase
    {
        private readonly string _baseDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private readonly string _ext = ".json";

        private readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override void Clear(string key)
        {
            var path = ResolvePath(key);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override bool Has(string key)
        {
            var path = ResolvePath(key);
            var result = File.Exists(path);
            return result;
        }

        public override void Initialize()
        {
            // Do nothing
        }

        public override byte[] Serialize<T>(T data)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, JsonSettings));
        }

        public override T Deserialize<T>(byte[] data)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data), JsonSettings);
        }

        private string ResolvePath(string itemName)
        {
            return Path.Combine(_baseDir, itemName + _ext);
        }

        public override void Write(string key, byte[] data)
        {
            File.WriteAllBytes(ResolvePath(key), data);
        }

        public override byte[] Read(string key)
        {
            return File.ReadAllBytes(ResolvePath(key));
        }
    }
}