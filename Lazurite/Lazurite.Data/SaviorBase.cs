namespace Lazurite.Data
{
    public abstract class SaviorBase: IDataManager
    {
        public abstract T Get<T>(string key);
        public abstract void Set<T>(string key, T data);
        public abstract void Clear(string key);
        public abstract bool Has(string key);
    }
}
