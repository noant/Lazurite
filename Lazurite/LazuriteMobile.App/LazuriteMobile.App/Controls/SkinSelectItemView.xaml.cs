
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SkinSelectItemView : Grid
    {
        public SkinSelectItemView (SkinBase skin)
        {
            InitializeComponent ();
            BindingContext = skin;
            if (Visual.Current.GetType() == skin.GetType())
            {
                lblCaption.Text = "Это текущий скин";
                btApply.InputTransparent = true;
                btApply.IsEnabled = false;
            }
        }

        private void BtApply_Click(object sender, EventArgs args)
        {
            Visual.ApplySkin(BindingContext as SkinBase);
        }
    }
}