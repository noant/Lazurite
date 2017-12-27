using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для MessageView.xaml
    /// </summary>
    public partial class MessageView : UserControl
    {
        private List<FrameworkElement> _tempDisabledElements = new List<FrameworkElement>();

        private Window _window;

        public MessageView()
        {
            InitializeComponent();
        }

        public void Show(Panel parentElement)
        {
            _tempDisabledElements.Clear();
            foreach (FrameworkElement element in parentElement.Children)
            {
                if (!element.IsEnabled)
                    _tempDisabledElements.Add(element);
                else element.IsEnabled = false;
            }
            parentElement.Children.Add(this);
            Panel.SetZIndex(this, 999);
        }

        public void Show()
        {
            var parent = Utils.GetMainWindowPanel();
            Show(parent);
        }

        public void ShowInNewWindow(Action shownCallback = null, int width = 500, bool showDialog=false)
        {
            var window = new Window();
            window.Name = "messageView";
            window.Height = 200;
            window.MinWidth = width;
            window.Width = width;
            gridSizable.MinWidth = width;
            gridSizable.Width = width;
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Topmost = true;
            window.ShowActivated = false;
            window.ShowInTaskbar = false;
            window.ContentRendered += (o,e) => shownCallback?.Invoke();
            window.Content = new Grid();
            _window = window;
            Show(window.Content as Grid);
            if (showDialog)
                window.ShowDialog();
            else
                window.Show();
        }

        public void Close()
        {
            if (_window != null)
            {
                _window.Close();
                _window = null;
            }
            else
            {
                this.Parent?.Dispatcher.BeginInvoke(new Action(() => {
                    foreach (FrameworkElement element in ((Panel)this.Parent).Children)
                    {
                        if (!_tempDisabledElements.Contains(element))
                            element.IsEnabled = true;
                    }
                    ((Panel)Parent).Children.Remove(this);
                }));
            }
        }

        public void SetItems(MessageItemInfo[] itemsInfos)
        {
            itemsView.Children.Clear();
            foreach (var item in itemsInfos)
            {
                var itemView = new ItemView() {
                    Content = item.Text,
                    Icon = item.Icon ?? Icon.Add,
                    IconVisibility = item.Icon != null ? Visibility.Visible : Visibility.Collapsed
                };
                itemView.MinWidth = 140;
                itemView.Margin = new Thickness(2,0,0,0);
                itemView.Click += (o, e) => item.Click?.Invoke(this);
                itemsView.Children.Add(itemView);
                if (item.Focused || (itemsInfos.All(x => !x.Focused) && itemsInfos.Last() == item))
                    itemView.Loaded += (o,e) => FocusManager.SetFocusedElement(this, itemView);
            }
        }

        public bool IsItemsEnabled
        {
            get
            {
                return itemsView.IsEnabled;
            }
            set
            {
                itemsView.IsEnabled = value;
            }
        }

        public string ContentText
        {
            get
            {
                return tbText.Text;
            }
            set
            {
                tbText.Text = value;
            }
        }

        public string HeaderText
        {
            get
            {
                return captionView.Content.ToString();
            }
            set
            {
                captionView.Content = value;
            }
        }

        public bool IsIconVisible
        {
            get
            {
                return iconView.Visibility != Visibility.Collapsed;
            }
            set
            {
                iconView.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Icon Icon
        {
            get
            {
                return iconView.Icon;
            }
            set
            {
                iconView.Icon = value;
            }
        }

        public void StartAnimateProgress()
        {
            captionView.StartAnimateProgress();
        }

        public void StopAnimateProgress()
        {
            captionView.StopAnimateProgress();
        }

        public static void ShowMessage(string message, string header, Icon icon, Panel parent = null, Action okCallback = null)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() => {
                if (parent == null)
                    parent = Utils.GetMainWindowPanel();
                var messageView = new MessageView();
                messageView.ContentText = message;
                messageView.HeaderText = header;
                messageView.Icon = icon;
                messageView.SetItems(new MessageItemInfo[] {
                new MessageItemInfo("OK",
                (v) => {
                    v.Close();
                    okCallback?.Invoke();
                }, Icon.Check, true)
            });
                messageView.Show(parent);
            }));
        }

        public static CancellationTokenSource ShowLoad(string message, Panel parent = null)
        {
            var tokenSource = new CancellationTokenSource();

            if (parent == null)
                parent = Utils.GetMainWindowPanel();

            parent.Dispatcher.BeginInvoke((Action)(() =>
            {
                var messageView = new MessageView();
                messageView.ContentText = message;
                messageView.HeaderText = "Пожалуйста, подождите...";
                messageView.Icon = Icons.Icon.MoonSleep;
                messageView.StartAnimateProgress();
                messageView.Show(parent);

                tokenSource.Token.Register(() => messageView.Dispatcher.BeginInvoke(new Action(() =>
                {
                    messageView.Close();
                })));
            }));

            return tokenSource;
        }

        public static void ShowYesNo(string message, string header, Icon icon, Action<bool> callback = null, Panel parent = null)
        {
            if (parent == null)
                parent = Utils.GetMainWindowPanel();
            var messageView = new MessageView();
            messageView.ContentText = message;
            messageView.HeaderText = header;
            messageView.Icon = icon;
            messageView.SetItems(new MessageItemInfo[] {
                new MessageItemInfo(
                "Да", 
                (v) =>
                {
                    v.Close();
                    callback?.Invoke(true);
                }, 
                Icon.Check),
                new MessageItemInfo(
                "Нет", 
                (v) =>
                {
                    v.Close();
                    callback?.Invoke(false);
                }, 
                Icon.Cancel, 
                true)
            });
            messageView.Show(parent);
        }
    }
}
