using ProtoBuf;
using SimpleRemoteMethods.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lazurite.MainDomain
{
    /// <summary>
    /// For protobuf serialization
    /// </summary>
    [ProtoContract]
    public class DictionaryCrutch
    {
        [ProtoMember(1, OverwriteList = true)]
        public KeyedPairs Data { get; set; }

        public Dictionary<string, object> GetNormalDictionary()
        {
            if (Data?.Count == 0) return new Dictionary<string, object>();

            var dict = new Dictionary<string, object>();
            foreach (var item in Data)
                dict.Add(item.Key, item.GetOriginalValue());
            return dict;
        }

        public DictionaryCrutch()
        {
            // Do nothing
        }

        public DictionaryCrutch(Dictionary<string, object> dict)
        {
            Data = new KeyedPairs();
            Data.AddRange(dict.Select(x => new KeyedPair(x.Key, x.Value)));
        }
    }
    
    [ProtoContract]
    public class KeyedPair
    {
        public KeyedPair()
        {
            // Do nothing 
        }

        public KeyedPair(string key, object value)
        {
            Key = key;
            ValuePB = ProtobufPrimitivesCreator.CreateSurrogate(value);
        }

        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2, DynamicType = true)]
        public object ValuePB { get; set; }

        public object GetOriginalValue() => ProtobufPrimitivesCreator.ExtractFromSurrogate(ValuePB);
    }

    [ProtoContract]
    public class KeyedPairs: List<KeyedPair>
    {

    }
}
