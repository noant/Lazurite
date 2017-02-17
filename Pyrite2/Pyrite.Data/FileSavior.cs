using System;
using HierarchicalData;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Pyrite.Utils;

namespace Pyrite.Data
{
    public class FileSavior : ISavior
    {
        private string _dir = "data";

        public T Get<T>(string key)
        {
            return HObject.FromFile(Path.Combine(_dir, key + ".xml")).Zero;
        }

        public void Set<T>(string key, T data)
        {
            var hobj = new HObject(Path.Combine(_dir, key + ".xml"));
            hobj.Zero = data;
            hobj.SaveToFile();
        }

        public void Clear(string key)
        {
            FileIO.
        }
    }
}
