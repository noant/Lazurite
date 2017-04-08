using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PyriteUI.Controls
{
    class ItemViewSelectionChangedEventArgs: RoutedEventArgs
    {
        public ItemView ItemView { get; internal set; }
        public bool Selected { get; internal set; }
    }
}
