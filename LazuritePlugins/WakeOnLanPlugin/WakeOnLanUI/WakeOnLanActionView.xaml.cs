using System;
using System.Windows.Controls;
using WakeOnLanUtils;

namespace WakeOnLanUI
{
    /// <summary>
    /// Логика взаимодействия для WakeOnLanActionView.xaml
    /// </summary>
    public partial class WakeOnLanActionView : UserControl
    {
        private IWakeOnLanAction _action;

        public WakeOnLanActionView()
        {
            InitializeComponent();
            searchView.AddressSelected += (address) => paramsView.SetMac(LanUtils.MacAddressParse(address.MacAddress.GetAddressBytes()));
            paramsView.ErrorStateChanged += (err) => btApply.IsEnabled = !err;
            btApply.Click += (o, e) => {
                paramsView.SetSettingsTo(_action);
                Apply?.Invoke();
            };
            btCancel.Click += (o, e) => Cancel?.Invoke();
        }

        public void RefreshWith(IWakeOnLanAction action)
        {
            paramsView.RefreshWith(_action = action);
        }

        public event Action Apply;
        public event Action Cancel;
    }
}
