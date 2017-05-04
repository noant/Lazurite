using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LazuriteUI.Windows.Controls
{
    class SelectionChangedEventArgs: RoutedEventArgs
    {
        public ISelectable Item { get; internal set; }
        public bool Selected { get; internal set; }
    }
}
