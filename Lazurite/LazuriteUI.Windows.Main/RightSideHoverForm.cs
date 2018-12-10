using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace LazuriteUI.Windows.Main
{
    public partial class RightSideHoverForm : Form
    {
        private static RightSideHoverForm RightSideHover = new RightSideHoverForm();

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public RightSideHoverForm()
        {
            InitializeComponent();

            Opacity = 0.01;

            StartPosition = FormStartPosition.Manual;

            Location = new System.Drawing.Point(
                Screen.PrimaryScreen.Bounds.Width - 4,
                (int)(Screen.PrimaryScreen.Bounds.Height * 0.8) - 20);
            
            Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.2) - 20;

            MouseEnter += RightSideHoverForm_MouseEnter;

            HandleCreated += (o,e) => SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        private void RightSideHoverForm_MouseEnter(object sender, EventArgs e) => 
            NotifyIconManager.ShowFastSwitchWindow();

        protected override CreateParams CreateParams
        {
            get
            {
                var @params = base.CreateParams;
                @params.ExStyle |= 0x80;
                return @params;
            }
        }

        public static void ShowWindow() => RightSideHover.Show();

        public static void HideWindow() => RightSideHover.Hide();

        public static void Initialize()
        {
            if (UISettings.Current.MouseRightSideHoverEvent)
                ShowWindow();
            else HideWindow();
        }
    }
}
