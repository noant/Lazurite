using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class ToggleView : ContentView
    {
        public ToggleView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                var a = 3;
            }
        }

        public ToggleView(ScenarioInfo scenario, UserVisualSettings visualSettings) : this()
        {
            this.BindingContext = new ScenarioModel(scenario, visualSettings);
        }
    }
}
