using Lazurite.Windows.Logging;
using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace LazuriteUI.Windows.Main.Journal
{
    /// <summary>
    /// Логика взаимодействия для JournalView.xaml
    /// </summary>
    [DisplayName("Журнал")]
    [LazuriteIcon(Icon.Book)]
    public partial class JournalView : UserControl
    {
        private static JournalView _current;

        public JournalView()
        {
            InitializeComponent();
            _current = this;
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Path.Combine(Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(JournalView).Assembly), "Logs"));
        }
        
        private void InternalSet(string message, WarnType type)
        {
            if (stackPanel.Children.Count == 200)
                stackPanel.Children.Clear();
            var itemView = new JournaltemView();
            itemView.DataContext = new JournalItemViewModel(message, type);
            stackPanel.Children.Add(itemView);
            scrollView.ScrollToEnd();
        }

        public static void Set(string message, WarnType type)
        {
            _current?.Dispatcher.BeginInvoke(new Action(() => {
                _current.InternalSet(message, type);
            }));
        }
    }
}
