using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Data
{
    public interface ISavior
    {
        T Get<T>(string key);
        void Set<T>(string key, T data);
    }
}
