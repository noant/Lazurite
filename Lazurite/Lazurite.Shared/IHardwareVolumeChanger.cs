namespace Lazurite.Shared
{
    public interface IHardwareVolumeChanger
    {
        event EventsHandler<int> VolumeUp;

        event EventsHandler<int> VolumeDown;
    }
}