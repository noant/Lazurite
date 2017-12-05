namespace WakeOnLanUtils
{
    public interface IWakeOnLanAction
    {
        string MacAddress { get; set; }

        ushort Port { get; set; }

        ushort TryCount { get; set; }
    }
}
