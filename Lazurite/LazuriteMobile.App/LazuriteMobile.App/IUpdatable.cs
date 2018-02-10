using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.App
{
    public interface IUpdatable
    {
        void UpdateView(Action callback);
    }
}
