using System.IO;

namespace Lazurite.Data
{
    public interface IDataManager
    {
        T Get<T>(string key);
        void Set<T>(string key, T data);

        void Clear(string key);
        bool Has(string key);
    }
}
