using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHost.Bases
{
    public interface IDataManager
    {
        void Save<T>(string name, T data);
        T Load<T>(string name);
        bool Exists(string name);
    }
}
