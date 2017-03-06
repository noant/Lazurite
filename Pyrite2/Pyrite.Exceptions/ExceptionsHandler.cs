using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Exceptions
{
    public class ExceptionsHandler: IExceptionsHandler
    {
        public void Handle(object sender, Action action, bool warning=false)
        {
            try
            {
                action();
            }
            catch(Exception e)
            {
                ExceptionThrown?.Invoke(sender, e, warning);
#if DEBUG
                if (!warning)
                    throw e;
#endif
            }
        }

        public event Action<object, Exception, bool> ExceptionThrown;
    }
}
