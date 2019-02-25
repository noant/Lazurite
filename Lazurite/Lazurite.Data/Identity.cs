using Lazurite.IOC;
using System;

namespace Lazurite.Data
{
    public static class Identity
    {
        private static readonly DataManagerBase DataManager = Singleton.Resolve<DataManagerBase>();
        private static string _id;

        /// <summary>
        /// Unique identificator for current Lazurite instance
        /// </summary>
        public static string UniqueId
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    if (DataManager.Has(nameof(Identity)))
                    {
                        _id = DataManager.Get<string>(nameof(Identity));
                    }
                    else
                    {
                        _id = Guid.NewGuid().ToString();
                        DataManager.Set(nameof(Identity), _id);
                    }
                }
                return _id;
            }
        }
    }
}