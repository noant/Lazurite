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

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += TapGesture_Tapped;
            gridBackground.GestureRecognizers.Add(tapGesture);
        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            Close();
        }

        public void Show(Grid parentElement)
        {
            parentElement.Children.Add(this);
            AllOpened.Add(this);
        }

        public void Close()
        {
            Closed?.Invoke(this, new EventArgs());
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

        public static bool AnyOpened
        {
            get
            {
                return AllOpened.Any();
            }
        }

        public static void CloseLast()
        {
            if (AnyOpened)
                AllOpened.Last().Close();
        }
    }
}
