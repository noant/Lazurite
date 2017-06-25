using Lazurite.Windows.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LazuriteUI.Windows.Main.Journal
{
    /// <summary>
    /// Логика взаимодействия для JournalWindow.xaml
    /// </summary>
    public partial class JournalLightWindow : Window
    {
        private static JournalLightWindow _current;
        private static bool _closed;
        private static object _locker = new object();

        private JournalLightWindow()
        {
            InitializeComponent();
            this.Top = 5;
            this.Left = 5;
            _closed = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            _closed = true;
            base.OnClosed(e);
        }

        public static void Show(string message, WarnType type)
        {
            lock (_locker)
            {
                if (_current == null || _closed)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _current = new JournalLightWindow();
                        _current.Show();
                        _current.Set(message, type);
                    }));
                }
                else
                    _current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _current.Set(message, type);
                    }));
            }
        }

        private void Set(string message, WarnType type)
        {
            progressView.StartProgress();
            if (stackPanel.Children.Count == 10)
            {
                stackPanel.Children.Clear();
                this.Height = 50;
            }
            var itemView = new JournaltemView();
            itemView.DataContext = new JournalItemViewModel(message, type);
            stackPanel.Children.Add(itemView);
            this.Height += itemView.Height;
            progressView.StopProgress();
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
