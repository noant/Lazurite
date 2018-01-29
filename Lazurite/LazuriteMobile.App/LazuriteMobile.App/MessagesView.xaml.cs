using Lazurite.IOC;
using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagesView : Grid, IUpdatable
    {
        private SynchronizationContext _currentContext = SynchronizationContext.Current;

        public MessagesView()
        {
            InitializeComponent();
        }

        public void UpdateMessages()
        {
            var scenariosManager = Singleton.Resolve<LazuriteContext>().Manager;
            scenariosManager.GetNotifications((notifications) => {
                _currentContext.Post(new SendOrPostCallback((s) => RefreshWith(notifications)), null);
            });
        }

        public void UpdateView() => UpdateMessages();

        private void RefreshWith(LazuriteNotification[] notifications)
        {            
            stackView.Children.Clear();
            foreach (var note in notifications)
                stackView.Children.Add(new MessageView(note));
            lblEmpty.IsVisible = !stackView.Children.Any();
        }
    }
}