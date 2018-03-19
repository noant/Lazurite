using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SliderWrapperView : ContentView
    {
        public SliderWrapperView()
        {
            InitializeComponent();
        }

        public double Value
        {
            get => sliderView.Value;
            set => sliderView.Value = value;
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged
        {
            add => sliderView.ValueChanged += value;
            remove => sliderView.ValueChanged -= value;
        }

        public double Maximum
        {
            get => sliderView.Maximum;
            set => sliderView.Maximum = value;
        }

        public double Minimum
        {
            get => sliderView.Minimum;
            set => sliderView.Minimum = value;
        }
    }
}