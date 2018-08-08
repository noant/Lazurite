using LazuriteMobile.App.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases
{
    public class LabelValueView : Label
    {
        public LabelValueView()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            TextColor = Visual.CaptionForeground;
            VerticalOptions = new LayoutOptions(LayoutAlignment.End, true);
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
            InputTransparent = true;
            FontSize = 11;
            FontFamily = Visual.FontFamily;
        }
    }
}
