using System.Threading;

namespace Lazurite.MainDomain
{
    public interface ISystemUtils
    {
        void Sleep(int ms, CancellationToken cancelToken);
    }
}
