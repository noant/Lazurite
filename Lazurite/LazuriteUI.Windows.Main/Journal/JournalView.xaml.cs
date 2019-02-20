using Lazurite.IOC;
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
        private static readonly WarningHandler Logger = Singleton.Resolve<WarningHandler>();
        private static JournalView _current;

        public JournalView()
        {
            InitializeComponent();
            RefreshWarnTypeButtons();
            btWarnTypeSelect.Click += (o, e) =>
            {
                SelectWarnTypeView.Show((warnType) =>
                {
                    JournalManager.MaxShowingWarnType = warnType;
                    RefreshWarnTypeButtons();
                });
            };
            btWarnTypeSelect_ToWrite.Click += (o, e) =>
            {
                SelectWarnTypeView.Show((warnType) =>
                {
                    Logger.MaxWritingWarnType = warnType;
                    RefreshWarnTypeButtons();
                });
            };
            _current = this;
        }

        private void RefreshWarnTypeButtons()
        {
            btWarnTypeSelect.Content = Enum.GetName(typeof(WarnType), JournalManager.MaxShowingWarnType);
            btWarnTypeSelect_ToWrite.Content = Enum.GetName(typeof(WarnType), Logger.MaxWritingWarnType);
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Path.Combine(Lazurite.Windows.Utils.Utils.GetAssemblyFolder(typeof(JournalView).Assembly), "Logs"));
        }

        private void InternalSet(string message, WarnType type)
        {
            if (stackPanel.Children.Count == 200)
            {
                stackPanel.Children.RemoveRange(0, 100);
            }

            var itemView = new JournaltemView();
            itemView.DataContext = new JournalItemViewModel(message, type);
            stackPanel.Children.Add(itemView);
            scrollView.ScrollToEnd();
        }

        public static void Set(string message, WarnType type)
        {
            _current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                _current.InternalSet(message, type);
            }));
        }
    }
}