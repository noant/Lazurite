using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class EntryView: Entry
    {
        public EntryView()
        {
            InitializeComponent();
        }

        public virtual void InitializeComponent()
        {
            TextColor = Visual.Current.Foreground;
            FontSize = Visual.Current.FontSize;
            FontFamily = Visual.Current.FontFamily;
            Margin = new Thickness(0, -1, 0, 1);
        }
    }
}
