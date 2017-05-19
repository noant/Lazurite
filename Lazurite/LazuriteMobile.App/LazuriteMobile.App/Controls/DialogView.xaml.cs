using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.contentGrid.Children.Add(child);
        }

        public void Show(Grid parentElement)
        {
            parentElement.Children.Add(this);
        }

        public void Close()
        {
            ((Grid)Parent).Children.Remove(this);
            Closed?.Invoke(this, new EventArgs());
        }

        private void CloseItemView_Click(object sender, EventArgs e)
        {
            Close();
        }

        public event Action<object, EventArgs> Closed;
    }
}
