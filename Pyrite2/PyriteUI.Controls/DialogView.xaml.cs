using PyriteUI.Icons;
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

namespace PyriteUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для MessageView.xaml
    /// </summary>
    public partial class DialogView : UserControl
    {
        private List<FrameworkElement> _tempDisabledElements = new List<FrameworkElement>();

        public DialogView(FrameworkElement child)
        {
            InitializeComponent();
            this.contentGrid.Children.Add(child);
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
