using Lazurite.IOC;
using LazuriteMobile.MainDomain;
using System;
using System.Linq;
using System.Threading;

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

        public void UpdateMessages(Action callback = null)
        {
            var scenariosManager = Singleton.Resolve<LazuriteContext>().Manager;
            scenariosManager.GetNotifications((notifications) => {
                _currentContext.Post(new SendOrPostCallback((s) =>
                {
                    RefreshWith(notifications);
                    callback?.Invoke();
                }), null);
            });
        }

        public void UpdateView(Action callback) => UpdateMessages(callback);

        private void RefreshWith(LazuriteNotification[] notifications)
        {
            BatchBegin();
            stackView.Children.Clear();
            foreach (var note in notifications)
                stackView.Children.Add(new MessageView(note));
            lblEmpty.IsVisible = !stackView.Children.Any();
            scrollView.ScrollToAsync(0, 0, false);
            BatchCommit();
        }
    }
}