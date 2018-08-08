using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases
{
    public class SwitchBase: Grid
    {
        public SwitchBase()
        {
            InitilizeComponent();
        }

        public void InitilizeComponent()
        {
            HeightRequest = 85;
            WidthRequest = 110;
            VerticalOptions = new LayoutOptions(LayoutAlignment.Start, true);
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, true);
                        
            Children.Add(new SwitchBottomBorder());
        }
    }
}
