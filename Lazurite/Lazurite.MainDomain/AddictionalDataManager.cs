using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public class AddictionalDataManager: IDisposable
    {
        public static readonly ILogger Log = Singleton.Resolve<ILogger>();

        private List<IAddictionalDataHandler> _handlers = new List<IAddictionalDataHandler>();

        public bool Any<T>() => _handlers.Any(x => x is T);

        public void Register<T>() where T: IAddictionalDataHandler
        {
            if (!_handlers.Any(x=> x is T))
                lock (_handlers)
                {
                    try
                    {
                        var handler = (IAddictionalDataHandler)Activator.CreateInstance<T>();
                        handler.Initialize();
                        _handlers.Add(handler);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat(e, "Error while initializing [{0}] data", typeof(T).Name);
                    }
                }
        }

        public void Unregister<T>() where T : IAddictionalDataHandler
        {
            lock (_handlers)
            {
                var handlers = _handlers.Where(x => x is T);
                foreach (var handler in handlers)
                {
                    if (handler is IDisposable)
                        ((IDisposable)handler).Dispose();
                }
            }
        }

        public void Handle(AddictionalData data)
        {
            foreach (var handler in _handlers)
                try
                {
                    handler.Handle(data);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(e, "Error while handling [{0}] data", handler.GetType().Name);
                }
        }

        public AddictionalData Prepare()
        {
            var data = new AddictionalData();
            foreach (var handler in _handlers)
            {
                try
                {
                    handler.Prepare(data);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(e, "Error while preparing [{0}] data", handler.GetType().Name);
                }
            }
            return data;
        }

        public void Dispose()
        {
            foreach (var handler in _handlers)
            {
                if (handler is IDisposable)
                    ((IDisposable)handler).Dispose();
            }
            _handlers = null;
        }
    }
}
