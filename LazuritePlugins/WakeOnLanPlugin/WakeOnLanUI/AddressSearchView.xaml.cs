using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WakeOnLanUtils;
using static WakeOnLanUtils.LanUtils;

namespace WakeOnLanUI
{
    /// <summary>
    /// Логика взаимодействия для AddressSearchView.xaml
    /// </summary>
    public partial class AddressSearchView : UserControl
    {
        private CancellationTokenSource _tokenSource = null;

        public AddressSearchView()
        {
            InitializeComponent();
            btSelect.IsEnabled = false;
            btSearch.Click += (o, e) => {
                captionView.StartAnimateProgress();
                _tokenSource?.Cancel();
                lvAddresses.Children.Clear();
                CancellationTokenSource oldTokenSource = null;
                oldTokenSource = _tokenSource = LanUtils.ListAllHosts(
                    ipBaseView.IpBase,
                    (address) => {
                        if (oldTokenSource == _tokenSource) //if new search is not starts
                        {
                            lvAddresses.Dispatcher.BeginInvoke((Action)(() => {
                                var itemView = new ItemView();
                                itemView.Content = address.IPAddress + " (mac: " + LanUtils.MacAddressParse(address.MacAddress.GetAddressBytes()) + ")";
                                itemView.Tag = address;
                                itemView.Margin = new Thickness(2);
                                lvAddresses.Children.Add(itemView);
                            }));
                        }
                    },
                    () => {
                        captionView.StopAnimateProgress();
                    });
            };

            lvAddresses.SelectionChanged += (o, e) => {
                btSelect.IsEnabled = lvAddresses.GetSelectedItems().Any();
            };

            btSelect.Click += (o, e) => {
                AddressSelected?.Invoke((Address)((ItemView)lvAddresses.SelectedItem).Tag);
            };
        }

        public event Action<Address> AddressSelected;
    }
}
