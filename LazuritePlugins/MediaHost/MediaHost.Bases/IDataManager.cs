namespace MediaHost.Bases
{
    public interface IDataManager
    {
        bool Save<T>(string name, T data);

        bool TryLoad<T>(string name, out T val);
    }
}