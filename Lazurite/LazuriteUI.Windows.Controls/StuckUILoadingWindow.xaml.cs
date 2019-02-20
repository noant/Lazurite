using System;
using System.Threading;
using System.Windows;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для StuckUILoadingWindow.xaml
    /// </summary>
    public partial class StuckUILoadingWindow : Window, IDisposable
    {
        public StuckUILoadingWindow()
        {
            InitializeComponent();
        }

        public static void Show(string message, Action target)
        {
            var dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            var newUiThread = new Thread(() =>
            {
                var view = new StuckUILoadingWindow();
                view.tbCaption.Text = message;
                view.progressView.StartProgress();
                view.ContentRendered += (o, e) =>
                {
                    dispatcher.BeginInvoke(new Action(() =>
                    {
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

        public static StuckUILoadingWindow Loading(string message)
        {
            var view = new StuckUILoadingWindow();
            view.Show();
            view.tbCaption.Text = message;
            view.progressView.StartProgress();
            return view;
        }

        public void Dispose()
        {
            Close();
        }
    }
}