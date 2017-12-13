using Lazurite.Windows.Logging;
using LazuriteUI.Icons;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
            RefreshWarnTypeButton();
            this.btWarnTypeSelect.Click += (o, e) => {
                SelectWarnTypeView.Show((warnType) => {
                    JournalManager.MaxShowingWarnType = warnType;
                    RefreshWarnTypeButton();
                });
            };
            _current = this;
        }

        private void RefreshWarnTypeButton()
        {
            this.btWarnTypeSelect.Content = Enum.GetName(typeof(WarnType), JournalManager.MaxShowingWarnType);
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
