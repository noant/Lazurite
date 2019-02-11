
using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SkinSelectView : StackLayout
    {
        public SkinSelectView ()
        {
            InitializeComponent ();

            var skinBaseType = typeof(SkinBase);

            var types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => x != skinBaseType && skinBaseType.IsAssignableFrom(x));

            foreach (var type in types)
                Children.Add(new SkinSelectItemView(Activator.CreateInstance(type) as SkinBase));
        }
    }
}