using ProtoBuf;

namespace Lazurite.Shared
{
    [ProtoContract]
    public class Messages
    {
        [ProtoMember(1, OverwriteList = true)]
        public Message[] All { get; set; }
    }
}
