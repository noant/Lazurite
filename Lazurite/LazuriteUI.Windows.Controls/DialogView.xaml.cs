using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для MessageView.xaml
    /// </summary>
    public partial class DialogView : UserControl
    {
        private List<FrameworkElement> _tempDisabledElements = new List<FrameworkElement>();
        public bool ShowUnderCursor { get; set; } = false;

        public DialogView(FrameworkElement child)
        {
            InitializeComponent();
            KeyUp += DialogView_KeyUp;
            gridBackground.MouseLeftButtonDown += GridBackground_MouseLeftButtonDown;
            contentControl.Content = child;
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); //crutch; initial focus
        }
        
        public string Caption
        {
            get
            {
                return tbCaption.Text;
            }
            set
            {
                tbCaption.Text = value;
            }
        }

        private void GridBackground_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void DialogView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
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

            if (ShowUnderCursor)
            {
                dockControl.VerticalAlignment = VerticalAlignment.Top;
                dockControl.HorizontalAlignment = HorizontalAlignment.Left;
                Loaded += (o, e) => RefreshPosition();
            }
            DialogOpened?.Invoke(this, new EventsArgs<object>(contentControl.Content));
        }

        private void RefreshPosition()
        {
            var mainWindowPanel = Utils.GetMainWindowPanel();
            var mainWindon = Utils.GetMainWindow();
            var mousePosition = mainWindowPanel.PointFromScreen(Utils.GetMousePosition());
            var margin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);
            if (margin.Top < 0)
                margin.Top = 0;
            dockControl.Margin = 
                new Thickness(
                    margin.Left, 
                    margin.Top - Utils.GetWindowTopBorderWithCaption(mainWindon), 
                    0, 0);
        }

        public void Show()
        {
            var parent = Utils.GetMainWindowPanel();
            Show(parent);
        }
        
        public void Close()
        {
            if (Parent != null)
            {
                var parentPanel = (Panel)Parent;
                if (contentControl.Content is IDisposable disposable)
                    disposable.Dispose();
                foreach (FrameworkElement element in parentPanel.Children)
                {
                    if (!_tempDisabledElements.Contains(element))
                        element.IsEnabled = true;
                }
                parentPanel.Children.Remove(this);
                Closed?.Invoke(this, new RoutedEventArgs());
                DialogClosed?.Invoke(this, new EventsArgs<object>(contentControl.Content));
            }
        }

        private void CloseItemView_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        public event RoutedEventHandler Closed;

        public static event EventsHandler<object> DialogOpened;
        public static event EventsHandler<object> DialogClosed;
    }
}
