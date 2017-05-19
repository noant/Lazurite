using System;
using System.Collections.Generic;
using System.Text;

namespace LazuriteMobile.App.Controls
{
    public interface ISelectable
    {
        bool Selected { get; set; }
        bool Selectable { get; set; }
        event Action<object, EventArgs> SelectionChanged;
    }
}
