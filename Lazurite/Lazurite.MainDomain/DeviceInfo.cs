using ProtoBuf;

namespace Lazurite.MainDomain
{
    [ProtoContract]
    public class DeviceInfo
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }
}
