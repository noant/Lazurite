using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public partial class DialogView : ContentView
    {
        private View _child;
		public DialogView(View child)
		{
			InitializeComponent();
            _child = child;
            contentGrid.Children.Add(child);
		}

		public void Show(Grid parentElement)
        {
            parentElement.Children.Add(this);
			AllOpened.Add(this);
        }

		public void Close()
		{
            Closed?.Invoke(this, EventArgs.Empty);
            if (contentGrid.Children.FirstOrDefault() is IDisposable disposable)
                disposable.Dispose();
            ((Grid)Parent).Children.Remove(this);
            AllOpened.Remove(this);
		}

		private void CloseItemView_Click(object sender, EventArgs e)
		{
			Close();
		}

		public event Action<object, EventArgs> Closed;

		///static members
		
		private static List<DialogView> AllOpened = new List<DialogView>();

		public static bool AnyOpened => AllOpened.Any();

		public static void CloseLast() => AllOpened.LastOrDefault()?.Close();

        public static void CloseAllDialogs() {
            foreach (var dialog in AllOpened.ToArray())
                dialog.Close();
        }
	}
}
