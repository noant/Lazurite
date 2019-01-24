using Lazurite.Shared;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class FloatViewManualSwitch : Grid
    {
        public FloatViewManualSwitch(SwitchScenarioModel model)
        {
            InitializeComponent();
            BindingContext = model;
            numericEntry.Max = model.Max;
            numericEntry.Min = model.Min;
            numericEntry.Completed += (o, e) => ApplyClicked?.Invoke(this, new EventsArgs<string>(numericEntry.Text));
            itemViewApply.Click += (o,e) => ApplyClicked?.Invoke(this, new EventsArgs<string>(numericEntry.Text));
            SizeChanged += (o,e) => numericEntry.Focus(); //crutch
        }
        
        public event EventsHandler<string> ApplyClicked;
    }
}
