using HierarchicalData;
using System;
using System.IO;
using System.Reflection;

namespace Lazurite.Data
{
    public class FileDataManager : DataManagerBase
    {
        private readonly string _dir = "data";
        private readonly string _extension = ".xml";
        private readonly string _baseDir;

        public FileDataManager()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Path.GetFullPath(Uri.UnescapeDataString(uri.Path));
            _baseDir = Path.GetDirectoryName(path);
        }

        public override byte[] Read(string key)
        {
            return File.ReadAllBytes(ResolvePath(key));
        }

        public override void Write(string key, byte[] data)
        {
            File.WriteAllBytes(ResolvePath(key), data);
        }

        public override T Deserialize<T>(byte[] data)
        {
            return HObject.FromStream(new MemoryStream(data)).Zero;
        }
        
        public override byte[] Serialize<T>(T data)
        {
            var hobj = new HObject(new MemoryStream());
            hobj.Zero = data;
            hobj.SaveToStream();
            hobj.Stream.Seek(0, SeekOrigin.Begin);
            var buff = new byte[hobj.Stream.Length];
            hobj.Stream.Read(buff, 0, buff.Length);
            return buff;
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
