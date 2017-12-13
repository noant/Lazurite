using LazuriteUI.Windows.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches.SwitchSettings
{
    /// <summary>
    /// Логика взаимодействия для SwitchSettings.xaml
    /// </summary>
    public partial class SwitchSetting : UserControl
    {
        public SwitchSetting()
        {
            InitializeComponent();
        }

        private void ShowLoadDialog(Action action)
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            MessageView messageDialog;
            Task.Factory.StartNew(() =>
            {
                messageDialog = new MessageView();
                messageDialog.Icon = Icons.Icon.ImageSelect;
                messageDialog.HeaderText = "Иконки";
                messageDialog.ContentText = "Загрузка иконок...";
                messageDialog.ShowInNewWindow();
                messageDialog.StartAnimateProgress();
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    action();
                    messageDialog.Dispatcher.BeginInvoke(new Action(() => messageDialog.Close()));
                }));
            }, CancellationToken.None, TaskCreationOptions.None, scheduler);
        }

        private void ItemView1_Click(object sender, RoutedEventArgs e)
        {
            ShowLoadDialog(() => {
                var switchIconSelect = new SwitchIconSelect(((ScenarioModel)this.DataContext), false);
                var dialog = new DialogView(switchIconSelect);
                dialog.Caption = "Выберите иконку, которая будет отображаться над переключателем. Для поиска нужной иконки начните вводить текст в поле ввода.";
                switchIconSelect.OkClick += (o, args) => dialog.Close();
                dialog.Show();
            });
        }

        private void ItemView2_Click(object sender, RoutedEventArgs e)
        {
            ShowLoadDialog(() => {
                var switchIconSelect = new SwitchIconSelect(((ScenarioModel)this.DataContext), true);
                var dialog = new DialogView(switchIconSelect);
                dialog.Caption = "Выберите иконку, которая будет отображаться над переключателем. Для поиска нужной иконки начните вводить текст в поле ввода.";
                switchIconSelect.OkClick += (o, args) => dialog.Close();
                dialog.Show();
            });
        }
    }
}
