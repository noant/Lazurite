using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class CaptionView: Label
    {
        public CaptionView()
        {
            InitializeComponent();
        }

        public virtual void InitializeComponent()
        {
            InputTransparent = true;
            FontSize = Visual.Current.FontSize;
            TextColor = Visual.Current.Foreground;
            FontFamily = Visual.Current.FontFamily;
        }
    }
}
