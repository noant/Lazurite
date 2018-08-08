using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases
{
    public class SwitchIconView : Controls.IconView
    {
        public SwitchIconView()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            Opacity = 0.64;
            WidthRequest = 45;
            HeightRequest = 45;
            VerticalOptions = new LayoutOptions(LayoutAlignment.Center, true);
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
            InputTransparent = true;
        }
    }
}
