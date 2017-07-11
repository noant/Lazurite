using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LazuriteUI.Windows.Controls
{
    public class WheelScroll: ScrollViewer
    {
        static WheelScroll()
        {
            App.InitializeResources();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WheelScroll), new FrameworkPropertyMetadata(typeof(WheelScroll)));
        }

        public WheelScroll()
        {
            this.VerticalScrollBarVisibility =
                this.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }
    }
}