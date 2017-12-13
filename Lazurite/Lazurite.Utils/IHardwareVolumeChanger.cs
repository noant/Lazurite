using Lazurite.Shared;

namespace Lazurite.Utils
{
    public interface IHardwareVolumeChanger
    {
        event EventsHandler<int> VolumeUp; 
        event EventsHandler<int> VolumeDown;
    }
}
