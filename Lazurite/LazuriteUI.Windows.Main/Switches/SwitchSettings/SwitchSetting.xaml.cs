using LazuriteUI.Windows.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                messageDialog.Icon = Icons.Icon.ImageGallery;
                messageDialog.HeaderText = "Иконки";
                messageDialog.ContentText = "Загрузка иконок...";
                messageDialog.Show();
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
                switchIconSelect.OkClick += (o, args) => dialog.Close();
                dialog.Show(Window.GetWindow(this).Content as Grid);
            });
        }

        private void ItemView2_Click(object sender, RoutedEventArgs e)
        {
            ShowLoadDialog(() => {
                var switchIconSelect = new SwitchIconSelect(((ScenarioModel)this.DataContext), true);
                var dialog = new DialogView(switchIconSelect);
                switchIconSelect.OkClick += (o, args) => dialog.Close();
                dialog.Show(Window.GetWindow(this).Content as Grid);
            });
        }
    }
}
