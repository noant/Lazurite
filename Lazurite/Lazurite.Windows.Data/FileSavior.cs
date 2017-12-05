using HierarchicalData;
using System;
using System.IO;
using System.Reflection;

namespace Lazurite.Data
{
    public class FileSavior : SaviorBase
    {
        private string _dir = "data";
        private string _extension = ".xml";
        private string _baseDir;

        public FileSavior()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            var path = Path.GetFullPath(Uri.UnescapeDataString(uri.Path));
            _baseDir = Path.GetDirectoryName(path);
        }

        public override T Get<T>(string key)
        {
            return HObject.FromFile(ResolvePath(key)).Zero;
        }

        public override void Set<T>(string key, T data)
        {
            var hobj = new HObject(ResolvePath(key));
            hobj.Zero = data;
            hobj.SaveToFile();
        }

        public override void Clear(string key)
        {
            File.Delete(ResolvePath(key));
        }

        public override bool Has(string key)
        {
            return File.Exists(ResolvePath(key));
        }

        private string ResolvePath(string key)
        {
            return Path.Combine(_baseDir,_dir, key + _extension);
        }
    }
}
