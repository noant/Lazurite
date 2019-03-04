using Lazurite.IOC;
using Lazurite.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public partial class DialogView : ContentView
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();

        public static Grid GetDialogHost(Element view)
        {
            Grid grid = null;
            while (view != null)
            {
                view = view.Parent;
                if (view is Grid)
                {
                    grid = (Grid)view;

                    if (grid is IDialogViewHost)
                    {
                        return grid;
                    }
                }
            }
            return grid;
        }

        private View _child;

        public DialogView(View child)
        {
            InitializeComponent();
            _child = child;
            contentGrid.Children.Add(child);
        }

        public void Show(Grid parentElement, string category = null)
        {
            Category = category;
            Device.BeginInvokeOnMainThread(() =>
            {
                parentElement.Children.Add(this);
                if (parentElement is IDialogViewHost dvh)
                {
                    Grid.SetColumn(this, dvh.Column);
                    Grid.SetRow(this, dvh.Row);
                    Grid.SetColumnSpan(this, dvh.ColumnSpan);
                    Grid.SetRowSpan(this, dvh.RowSpan);
                }
            });
            AllOpened.Add(this);
        }

        public void Close()
        {
            try
            {
                Closed?.Invoke(this, EventArgs.Empty);
                if (contentGrid.Children.FirstOrDefault() is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    if (Parent is Grid grid && grid.Children != null)
                    {
                        grid.Children.Remove(this);
                    }
                });

                AllOpened.Remove(this);
            }
            catch (Exception e)
            {
                Log.Error(exception: e);
            }
        }

        private void CloseItemView_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string Category { get; set; }

        public event Action<object, EventArgs> Closed;

        ///static members

        private static List<DialogView> AllOpened = new List<DialogView>();

        public static bool AnyOpened => AllOpened.Any();

        public static void CloseLast() => AllOpened.LastOrDefault()?.Close();

        public static void CloseAllDialogs(string category = null, bool all = true)
        {
            foreach (var dialog in AllOpened.Where(x => all || x.Category == category).ToArray())
            {
                dialog.Close();
            }
        }
    }
}