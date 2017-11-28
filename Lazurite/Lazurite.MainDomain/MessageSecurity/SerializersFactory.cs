using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Lazurite.Utils;
using Lazurite.Logging;
using Lazurite.IOC;

namespace Lazurite.MainDomain.MessageSecurity
{
    public static class SerializersFactory
    {
        private static ILogger Log = Singleton.Resolve<ILogger>();
        private static object Locker_SerializatorCreating = new object();

        static SerializersFactory()
        {
            var allTypes = typeof(ValueTypeBase).GetTypeInfo().Assembly.DefinedTypes
                .Union(typeof(SerializersFactory).GetTypeInfo().Assembly.DefinedTypes)
                .Select(x=>x.AsType()).ToArray();

            var knownTypes = new List<Type>();

            foreach (var type in allTypes.Where(x => x.GetTypeInfo().GetCustomAttribute<DataContractAttribute>() != null))
            {
                var assignableTypes = ReflectionUtils.GetAllOfType(type, allTypes).Select(x=>x.AsType());
                knownTypes.AddRange(assignableTypes);
            }

            KnownTypes = knownTypes.ToArray();
        }

        private static Dictionary<Type, DataContractSerializer> Cached = new Dictionary<Type, DataContractSerializer>();
        private static Type[] KnownTypes;

        public static DataContractSerializer GetSerializer<T>()
        {
            var type = typeof(T);
            try
            {
                lock (Locker_SerializatorCreating)
                {
                    if (!Cached.ContainsKey(type))
                    {
                        var serializer = new DataContractSerializer(type, KnownTypes);
                        Cached.Add(type, serializer);
                        Log.DebugFormat("Serializer for type [{0}] created", type.FullName);
                    }
                }
                return Cached[type];
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Error while creating serializer for type [{0}];", type.FullName);
                throw e;
            }
        }
    }
}
