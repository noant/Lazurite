using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserGeolocationPlugin;

namespace UserGeolocationPluginUI
{
    /// <summary>
    /// Логика взаимодействия для PlaceEditView.xaml
    /// </summary>
    public partial class PlaceEditView : UserControl
    {
        private GeolocationPlace _place;

        public PlaceEditView()
        {
            InitializeComponent();
            tbRadious.Validation = EntryViewValidation.UIntValidation(max: 6371*1000); //earth radious
            tbPlaceName.TextChanged += (o, e) => UpdateControls();
            tbRadious.TextChanged += (o, e) =>
            {
                var value = int.Parse(tbRadious.Text);
                if (value > sliderRadious.Maximum)
                    sliderRadious.Maximum = value;
                sliderRadious.Value = int.Parse(tbRadious.Text);
                UpdateControls();
            };
            sliderRadious.ValueChanged += (o, e) =>
            {
                tbRadious.Text = ((int)sliderRadious.Value).ToString();
                UpdateControls();
            };
            btApply.Click += (o, e) =>
            {
                Apply();
            };
        }

        public void RefreshWith(GeolocationPlace place)
        {
            sliderRadious.Maximum = 3000;
            if (place != null)
            {
                _place = place;
                tbPlaceName.Text = _place.Name;
                tbRadious.Text = _place.MetersRadious.ToString();
                btApply.IsEnabled = false;
                this.IsEnabled = true;
            }
            else
                this.IsEnabled = false;
        }

        private void Apply()
        {
            _place.MetersRadious = int.Parse(tbRadious.Text);
            _place.Name = tbPlaceName.Text;
            PlacesManager.Save();
            SettingsApplied?.Invoke(this, new EventsArgs<GeolocationPlace>(_place));
        }

        private void UpdateControls()
        {
            btApply.IsEnabled = !PlacesManager.Current.Places.Any(x => x.Name.Equals(_place.Name) && !x.Location.Equals(_place.Location));
        }

        public event EventsHandler<GeolocationPlace> SettingsApplied;
    }
}