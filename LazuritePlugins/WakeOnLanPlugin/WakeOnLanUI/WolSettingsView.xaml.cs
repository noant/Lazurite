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
using WakeOnLanUtils;

namespace WakeOnLanUI
{
    /// <summary>
    /// Логика взаимодействия для WolSettingsView.xaml
    /// </summary>
    public partial class WolSettingsView : UserControl
    {
        public WolSettingsView()
        {
            InitializeComponent();
            tbTryCount.Validation = EntryViewValidation.UShortValidation(min: 1);
            tbPort.Validation = EntryViewValidation.UShortValidation();
            tbMac.Validation = (s, v) => {
                try
                {
                    LanUtils.MacAddressParse(s.Text);
                }
                catch
                {
                    v.ErrorMessage = "МAC-адрес должен состоять из шести\r\nшестнадцатеричных чисел (от 00 до FF), разделенных двоеточием";
                }
            };
            tbMac.ErrorStateChanged += (s) => {
                var isError = s.InputWrong;
                if (isError)
                    tbError.Text = s.ErrorMessage;
                else
                    tbError.Text = null;
                ErrorStateChanged?.Invoke(isError);
            };
        }

        public void RefreshWith(IWakeOnLanAction action)
        {
            tbMac.Text = action.MacAddress;
            tbPort.Text = action.Port.ToString();
            tbTryCount.Text = action.TryCount.ToString();
        }

        public void SetMac(string macAddress)
        {
            tbMac.Text = macAddress;
        }

        public void SetSettingsTo(IWakeOnLanAction action)
        {
            action.MacAddress = tbMac.Text;
            action.Port = ushort.Parse(tbPort.Text);
            action.TryCount = ushort.Parse(tbTryCount.Text);
        }

        public event Action<bool> ErrorStateChanged;
    }
}
