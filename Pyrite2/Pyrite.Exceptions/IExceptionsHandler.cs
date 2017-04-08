using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Exceptions
{
    public interface IExceptionsHandler
    {
        void Handle(object sender, Action action, bool warning=false);
        T Handle<T>(object sender, Func<T> action, bool warning=false);
    }
}
