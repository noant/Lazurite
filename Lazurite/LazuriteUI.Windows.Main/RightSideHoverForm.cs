using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace LazuriteUI.Windows.Main
{
    public partial class RightSideHoverForm : Form
    {
        private static RightSideHoverForm RightSideHover = new RightSideHoverForm();

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

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
                        SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
                    }));
                },
                null,
                1000 * 45,
                1000 * 45); // Каждые 45 секунд
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
                StartActivationTimer();
            else
                StopActivationTimer();
        }

        private void StopActivationTimer()
        {
            _activateTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void ShowWindow() {
            RightSideHover.Show();
        }

        public static void HideWindow() {
            RightSideHover.Hide();
        }

        public static void Initialize()
        {
            if (UISettings.Current.MouseRightSideHoverEvent)
                ShowWindow();
            else HideWindow();
        }
    }
}
