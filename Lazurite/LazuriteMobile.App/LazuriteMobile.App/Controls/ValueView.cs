using System;
using System.Collections.Generic;
using System.Text;

namespace LazuriteMobile.App.Controls
{
    public class ValueView: CaptionView
    {
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            TextColor = Visual.Current.ValueForeground;
        }
    }
}
