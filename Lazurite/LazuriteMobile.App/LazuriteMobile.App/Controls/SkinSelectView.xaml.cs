
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

            var skins = types
                .Select(x => Activator.CreateInstance(x) as SkinBase)
                .OrderBy(x => x.VisualOrder)
                .ToArray();

            foreach (var skin in skins)
                Children.Add(new SkinSelectItemView(skin));
        }
    }
}