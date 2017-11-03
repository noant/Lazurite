using System;
using HierarchicalData;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Lazurite.Windows.Utils;

namespace Lazurite.Data
{
    public class PluginsDataManager : PluginsDataManagerBase
    {
        private string _dir = "data";
        private string _extension = ".xml";
        private string _dirPrefix = "pluginData_";
        private string _baseDir;

        public PluginsDataManager()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            var path = Path.GetFullPath(Uri.UnescapeDataString(uri.Path));
            _baseDir = Path.GetDirectoryName(path);
        }

        public override T Get<T>(string key)
        {
            return HObject.FromFile(ResolvePath(key, Assembly.GetCallingAssembly())).Zero;
        }

        public override void Set<T>(string key, T data)
        {
            var hobj = new HObject(ResolvePath(key, Assembly.GetCallingAssembly()));
            hobj.Zero = data;
            hobj.SaveToFile();
        }

        public override void Clear(string key)
        {
            File.Delete(ResolvePath(key, Assembly.GetCallingAssembly()));
        }

        public override bool Has(string key)
        {
            return File.Exists(ResolvePath(key, Assembly.GetCallingAssembly()));
        }

        private string ResolvePath(string key, Assembly callingAssembly)
        {
            var pluginPath = Utils.GetAssemblyFolder(callingAssembly);
            var pluginName = new DirectoryInfo(pluginPath).Name;
            return Path.Combine(_baseDir, _dir, _dirPrefix + pluginName, key + _extension);
        }
    }
}
