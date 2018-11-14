using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Shared
{
    public interface IScenariosEnumerator
    {
        void SetCasts(Func<ScenarioCast[]> needCasts);
    }
}
