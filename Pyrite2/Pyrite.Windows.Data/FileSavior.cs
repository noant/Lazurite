using System;
using HierarchicalData;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Pyrite.Data
{
    public class FileSavior : ISavior
    {
        private string _dir = "data";
        private string _extension = ".xml";
        private string _baseDir;

        public FileSavior()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            _baseDir = Path.GetDirectoryName(path);
        }

        public T Get<T>(string key)
        {
            return HObject.FromFile(ResolvePath(key)).Zero;
        }

        public void Set<T>(string key, T data)
        {
            var hobj = new HObject(ResolvePath(key));
            hobj.Zero = data;
            hobj.SaveToFile();
        }

        public void Clear(string key)
        {
            File.Delete(ResolvePath(key));
        }

        private string ResolvePath(string key)
        {
            return Path.Combine(_baseDir,_dir, key + ".xml");
        }
    }
}
