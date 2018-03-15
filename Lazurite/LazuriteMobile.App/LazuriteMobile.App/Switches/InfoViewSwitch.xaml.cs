using Lazurite.Shared;
using LazuriteMobile.App.Controls;
using System;
using System.Reflection;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class InfoViewSwitch : ContentView
    {
        public InfoViewSwitch(SwitchScenarioModel model)
        {
            InitializeComponent();
            BindingContext = model;
            itemViewApply.Click += (o,e) => ApplyClicked?.Invoke(this, new EventsArgs<string>(tbText.Text));
            SizeChanged += (o,e) => tbText.Focus(); //crutch
            tbText.Keyboard = Keyboard.Chat;
            tbText.TextChanged += (o, e) => tbText.InvalidateMeasureCrutch(); //crutch
        }
        
        public event EventsHandler<string> ApplyClicked;

        public static void Show(SwitchScenarioModel model, Grid parent)
        {
            var @switch = new InfoViewSwitch(model);
            var dialog = new DialogView(@switch);
            @switch.ApplyClicked += (o, e) =>
            {
                dialog.Close();
                model.ScenarioValue = e.Value;
            };
            dialog.Show(parent);
        }
    }

    //crutch
    public class EditorExt: Editor
    { 
        //crutch
        public void InvalidateMeasureCrutch() => InvalidateMeasure();
    }
}
