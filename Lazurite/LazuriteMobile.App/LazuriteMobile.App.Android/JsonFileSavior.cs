using Lazurite.Data;
using Newtonsoft.Json;
using System;
using System.IO;

namespace LazuriteMobile.App.Droid
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2146:TypesMustBeAtLeastAsCriticalAsBaseTypesFxCopRule")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2132:DefaultConstructorsMustHaveConsistentTransparencyFxCopRule")]
    public class JsonFileSavior : SaviorBase
    {
        private readonly string _baseDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private readonly string _ext = ".json";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override void Clear(string key)
        {
            var path = GetPath(key);
            if (File.Exists(path))
                File.Delete(path);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override T Get<T>(string key)
        {
            var path = GetPath(key);
            var data = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override bool Has(string key)
        {
            var path = GetPath(key);
            var result = File.Exists(path);
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2134:MethodsMustOverrideWithConsistentTransparencyFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
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