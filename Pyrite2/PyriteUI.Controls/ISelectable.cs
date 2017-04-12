using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PyriteUI.Controls
{
    public interface ISelectable
    {
        bool Selected { get; set; }
        bool Selectable { get; set; }
        event RoutedEventHandler SelectionChanged;
    }
}
