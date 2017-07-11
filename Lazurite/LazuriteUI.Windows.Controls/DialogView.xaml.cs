using LazuriteUI.Icons;
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
            var mainWindow = Utils.GetMainWindow();
            var mousePosition = GetMousePosition();
            var margin = new Thickness(mousePosition.X, mousePosition.Y - dockControl.ActualHeight / 2, 0, 0);
            if (margin.Top < 0)
                margin.Top = 0;
            if (margin.Top + dockControl.ActualHeight > mainWindow.ActualHeight)
                dockControl.Height = mainWindow.ActualHeight - margin.Top;

            dockControl.Margin = new Thickness(margin.Left - ((Window)Utils.GetMainWindowPanel().Parent).Left, margin.Top - ((Window)Utils.GetMainWindowPanel().Parent).Top, 0, 0);
        }

        public System.Windows.Point GetMousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new System.Windows.Point(point.X, point.Y);
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
