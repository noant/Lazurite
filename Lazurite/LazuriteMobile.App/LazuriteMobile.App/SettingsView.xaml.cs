using Lazurite.MainDomain;
using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App
{
    public partial class SettingsView : ContentView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public void SetCredentials(ConnectionCredentials credentials)
        {
            tbHost.Text = credentials.Host;
            tbLogin.Text = credentials.Login;
            tbPassword.Text = credentials.Password;
            tbSecretCode.Text = credentials.SecretKey;
            tbService.Text = credentials.ServiceName;
            numPort.Value = credentials.Port;
        }

        public ConnectionCredentials GetCredentials()
        {
            return new ConnectionCredentials() {
                Host = tbHost.Text,
                Login = tbLogin.Text,
                Password = tbPassword.Text,
                SecretKey = tbSecretCode.Text,
                Port = (ushort)numPort.Value,
                ServiceName = tbService.Text
            };
        }

        public void SetErrorMessage(string str)
        {
            lblErrorMessage.Text = str;
        }

        public void ClearErrorMessage()
        {
            SetErrorMessage("");
        }

        private void itemView_Click(object arg1, EventArgs arg2)
        {
            ConnectClicked?.Invoke(this);
        }

        public event Action<SettingsView> ConnectClicked;
    }
}
