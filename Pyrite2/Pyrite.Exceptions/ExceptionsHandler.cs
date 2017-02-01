using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Exceptions
{
    public class ExceptionsHandler
    {
        public void Handle(object sender, Action action)
        {
            try
            {
                action();
            }
            catch(Exception e)
            {
                if (ExceptionThrown!=null)
                {
                    ExceptionThrown(sender, e);
                }
            }
        }

        public event Action<object, Exception> ExceptionThrown;
    }
}
