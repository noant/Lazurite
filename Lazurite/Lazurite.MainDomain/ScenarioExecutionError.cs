using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public enum ScenarioExecutionError
    {
        StackOverflow,
        CircularReference,
        InvalidValue,
        AccessDenied,
        NotAvailable
    }
}
