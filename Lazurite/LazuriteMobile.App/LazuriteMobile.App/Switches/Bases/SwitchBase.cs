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

            SetBinding(BackgroundColorProperty,
                new Binding(nameof(SwitchScenarioModel.AllowClick),
                converter: ConvertersStatic.Background_BoolToColor));

            SetBinding(OpacityProperty,
                new Binding(nameof(SwitchScenarioModel.IsAvailable),
                converter: ConvertersStatic.BoolToDouble));
        }
    }
}
