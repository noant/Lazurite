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

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для StuckUILoadingWindow.xaml
    /// </summary>
    public partial class StuckUILoadingWindow : Window
    {
        public StuckUILoadingWindow()
        {
            InitializeComponent();
        }
        
        public static void Show(string message, Action target)
        {
            var dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            var newUiThread = new Thread(() => {
                var view = new StuckUILoadingWindow();
                view.tbCaption.Text = message;
                view.progressView.StartProgress();
                view.ContentRendered += (o, e) =>
                {
                    dispatcher.BeginInvoke(new Action(() => {
                        target?.Invoke();
                        view.Dispatcher.BeginInvoke(new Action(view.Close));
                    }));
                };
                view.ShowDialog();
                System.Windows.Threading.Dispatcher.Run();
            });
            newUiThread.IsBackground = true;
            newUiThread.SetApartmentState(ApartmentState.STA);
            newUiThread.Start();
        }
    }
}
