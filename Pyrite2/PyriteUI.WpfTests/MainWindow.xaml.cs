using PyriteUI.Controls;
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

namespace PyriteUI.WpfTests
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            listItems.SelectionMode = Controls.ListViewItemsSelectionMode.Multiple;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            listItems.SelectionMode = Controls.ListViewItemsSelectionMode.Single;
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            listItems.SelectionMode = Controls.ListViewItemsSelectionMode.None;

            var messageView = new MessageView();
            messageView.Icon = Icons.Icon.ConfirmYesNo;
            messageView.ContentText = "Вы уверены, что хотите удалить сценарий 'Включить основной свет'?";
            messageView.HeaderText = "Удаление сценария";
            messageView.Show(grid);
            messageView.SetItems(new[] {
                new MessageItemInfo("Да", (mv) => { 
                    mv.ContentText = "Удаление сценария...";
                    mv.StartAnimateProgress();
                    mv.IsItemsEnabled = false;
                    Task.Delay(8000).ContinueWith((t) => {
                        mv.Dispatcher.BeginInvoke(new Action(()=> {
                            mv.StopAnimateProgress();
                            mv.ContentText = "Удаление завершено";
                            mv.Icon = Icons.Icon.PinRemove;
                            mv.SetItems(new [] {
                                new MessageItemInfo("Ок", (mv1) => mv1.Close(), Icons.Icon.Check),
                            });
                            mv.IsItemsEnabled = true;
                        }));
                    });
                }, Icons.Icon.Check),
                new MessageItemInfo("Нет", (mv) => mv.Close(), Icons.Icon.Cancel),
                new MessageItemInfo("Блабла", (mv) => mv.Close()),
            });
        }
    }
}
