using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Shared;
using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.App
{
    public class MessagesDataHandler : IAddictionalDataHandler
    {
        private INotifier _notifier;

        public void Handle(AddictionalData data, object tag)
        {
            try
            {
                if (_notifier != null)
                {
                    var messages = data.Resolve<Messages>();
                    if (messages?.All != null)
                        foreach (var message in messages.All)
                            _notifier.Notify(message);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Initialize()
        {
            if (Singleton.Any<INotifier>())
                _notifier = Singleton.Resolve<INotifier>();
        }

        public void Prepare(AddictionalData data, object tag)
        {
            //
        }
    }
}
