using ProtoBuf;
using System;

namespace Lazurite.Shared
{
    [ProtoContract]
    public class Message
    {
        [ProtoMember(1)]
        public DateTime DateTime { get; set; }
        [ProtoMember(2)]
        public string Text { get; set; }
        [ProtoMember(3)]
        public string Header { get; set; }
    }
}
