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
        public KeyedPair[] Data { get; set; }

        public Dictionary<string, object> GetNormalDictionary()
        {
            if (Data == null || Data.Length == 0)
                return new Dictionary<string, object>();

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
            Data = dict.Select(x => new KeyedPair(x.Key, x.Value)).ToArray();
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
            ValuePB = DynamicSurrogate.Create(value);
        }

        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public DynamicSurrogate ValuePB { get; set; }

        public object GetOriginalValue() => DynamicSurrogate.Extract(ValuePB);
    }
}
