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
            this.contentGrid.Children.Add(child);
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

            if (ShowUnderCursor)
            {
                contentGrid.VerticalAlignment = VerticalAlignment.Top;
                contentGrid.HorizontalAlignment = HorizontalAlignment.Left;
                this.Loaded += (o, e) => {
                    var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                    var mouse = transform.Transform(GetMousePosition());
                    var margin = new Thickness(mouse.X, mouse.Y - contentGrid.ActualHeight / 2, 0, 0);

                    if (margin.Top + contentGrid.ActualHeight > this.ActualHeight)
                        margin.Top = 0;
                    if (margin.Top < 0)
                        margin.Top = 0;

                    contentGrid.Margin = new Thickness(mouse.X - ((Window)Utils.GetMainWindowPanel().Parent).Left, mouse.Y - contentGrid.ActualHeight/2 - ((Window)Utils.GetMainWindowPanel().Parent).Top, 0, 0);
                };
            }

            parentElement.Children.Add(this);
            Panel.SetZIndex(this, 999);
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
            foreach (FrameworkElement element in ((Panel)this.Parent).Children)
            {
                if (!_tempDisabledElements.Contains(element))
                    element.IsEnabled = true;
            }
            ((Panel)Parent).Children.Remove(this);
            Closed?.Invoke(this, new RoutedEventArgs());
        }

        private void CloseItemView_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        public event RoutedEventHandler Closed;
    }
}
