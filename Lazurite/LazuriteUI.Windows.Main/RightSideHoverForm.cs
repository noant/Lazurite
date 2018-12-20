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
using System.Threading;

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

        private System.Threading.Timer _activateTimer;
        
        public RightSideHoverForm()
        {
            InitializeComponent();
            Opacity = 0.01;
            StartPosition = FormStartPosition.Manual;
            RefreshLocationAndSize();
            MouseEnter += RightSideHoverForm_MouseEnter;
            HandleCreated += (o,e) => SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        private void RefreshLocationAndSize()
        {
            Location = new System.Drawing.Point(
                Screen.PrimaryScreen.Bounds.Width - 4,
                (int)(Screen.PrimaryScreen.Bounds.Height * 0.8) - 20);

            Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.2) - 20;
        }

        private void RightSideHoverForm_MouseEnter(object sender, EventArgs e) => 
            NotifyIconManager.ShowFastSwitchWindow();

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                var @params = base.CreateParams;
                @params.ExStyle |= 0x80;
                return @params;
            }
        }

        private void StartActivationTimer()
        {
            _activateTimer = new System.Threading.Timer(
                (s) =>
                {
                    BeginInvoke(new Action(() => {
                        Hide(); // Нужно для периодического помещения окна на передний план, если его загородило новое TopMost окно
                        RefreshLocationAndSize(); // На тот случай, если пользователь поменял разрешение экрана или подключил другой монитор
                        Show();
                    }));
                },
                null,
                1000 * 60 * 4,
                1000 * 60 * 4); // Каждые 4 минуты
        }

        private void StopActivationTimer()
        {
            _activateTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void ShowWindow() {
            RightSideHover.Show();
            RightSideHover.StartActivationTimer();
        }

        public static void HideWindow() {
            RightSideHover.Hide();
            RightSideHover.StopActivationTimer();
        }

        public static void Initialize()
        {
            if (UISettings.Current.MouseRightSideHoverEvent)
                ShowWindow();
            else HideWindow();
        }
    }
}
