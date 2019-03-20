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
        private string _targetFolder;

        public override void Initialize()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Path.GetFullPath(Uri.UnescapeDataString(uri.Path));
            var baseDir = Path.GetDirectoryName(path);

            _targetFolder = Path.Combine(baseDir, _dir);

            if (!Directory.Exists(_targetFolder))
                Directory.CreateDirectory(_targetFolder);
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
            using (var ms = new MemoryStream())
            {
                var hobj = new HObject();
                hobj.Zero = data;
                hobj.SaveToStream(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var buff = new byte[ms.Length];
                ms.Read(buff, 0, buff.Length);
                return buff;
            }
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
            return Path.Combine(_targetFolder, key + _extension);
        }
    }
}