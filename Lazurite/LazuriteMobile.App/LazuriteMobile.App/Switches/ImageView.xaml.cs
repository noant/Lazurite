using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class ImageView : ContentView
    {
        public ImageView()
        {
            InitializeComponent();
        }

        public ImageView(ScenarioInfo scenario) : this()
        {
            this.BindingContext = new SwitchScenarioModel(scenario);
        }
    }
}
