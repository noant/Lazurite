using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main
{
    public interface IAllowSave
    {
        void Save(Action callback);
    }
}
