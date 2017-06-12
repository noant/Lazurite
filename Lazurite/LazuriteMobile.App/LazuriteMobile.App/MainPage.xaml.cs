using Lazurite.IOC;
using Lazurite.MainDomain.MessageSecurity;
using LazuriteMobile.App.Controls;
using LazuriteMobile.App.Switches;
using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App
{
	public partial class MainPage : ContentPage
	{
        IScenariosManager _manager = Singleton.Resolve<IScenariosManager>();
        
        SynchronizationContext _currentContext = SynchronizationContext.Current;

        public MainPage()
		{
            this.InitializeComponent();
            _manager.NeedRefresh += _manager_NeedRefresh;
            _manager.ConnectionLost += _manager_ConnectionLost;
            _manager.ConnectionRestored += _manager_ConnectionRestored;
        }

        public void HideCaption()
        {
            this.gridCaption.IsVisible = false;
        }

        public void ShowCaption(string text, bool error = false)
        {
            this.gridCaption.IsVisible = true;
            lblCaption.Text = text;
            lblCaption.BackgroundColor = error ? Color.Red : Color.White;
        }

        private void _manager_ConnectionRestored()
        {
            _currentContext.Post((state) => {
                swgrid.IsEnabled = true;
            }, null);
        }

        private void _manager_ConnectionLost()
        {
            _currentContext.Post((state) => {
                swgrid.IsEnabled = false;
            }, null);
        }

        private void _manager_NeedRefresh()
        {
            _currentContext.Post((state) => {
                HideCaption();
                swgrid.Refresh(_manager.Scenarios);
            }, null);
        }        
    }
}