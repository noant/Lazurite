using LazuriteMobile.App.Controls;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases
{
    public class SwitchValueView : ValueView
    {
        public override void InitializeComponent()
        {
            base.InitializeComponent();

            VerticalOptions = new LayoutOptions(LayoutAlignment.End, true);
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);

            SetBinding(TextColorProperty,
                new Binding(nameof(SwitchScenarioModel.State), 
                converter: ConvertersStatic.ValueForeground_StateToColor));
        }
    }
}