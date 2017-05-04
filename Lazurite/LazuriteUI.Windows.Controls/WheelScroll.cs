using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LazuriteUI.Windows.Controls
{
    public class WheelScroll: ScrollViewer
    {
        public WheelScroll()
        {
            this.IsHitTestVisible = true;
            this.Background = Brushes.Transparent;
            this.ClipToBounds = true;
            this.VerticalScrollBarVisibility =
                this.HorizontalScrollBarVisibility =
                ScrollBarVisibility.Hidden;
        }
    }
}