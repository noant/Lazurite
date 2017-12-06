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
            this.KeyUp += DialogView_KeyUp;
            this.gridBackground.MouseLeftButtonDown += GridBackground_MouseLeftButtonDown;
            this.contentControl.Content = child;
            this.Loaded += (sender, e) =>
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
            this.Close();
        }

        private void DialogView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
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
                this.Loaded += (o, e) => RefreshPosition();
            }
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
            if (this.Parent != null)
            {
                if (this.contentControl.Content is IDisposable)
                    ((IDisposable)this.contentControl.Content).Dispose();
                foreach (FrameworkElement element in ((Panel)this.Parent).Children)
                {
                    if (!_tempDisabledElements.Contains(element))
                        element.IsEnabled = true;
                }
                ((Panel)Parent).Children.Remove(this);
                Closed?.Invoke(this, new RoutedEventArgs());
            }
        }

        private void CloseItemView_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        public event RoutedEventHandler Closed;
    }
}
