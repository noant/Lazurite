using Lazurite.Shared;
using MediaHost.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace MediaHost.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string className, string windowText);

        public IReadOnlyCollection<MediaPanelBase> Sources
        {
            get => _sources;
            set
            {
                if (DataManager == null)
                {
                    throw new ArgumentNullException("DataManager должен быть установлен");
                }

                foreach (var source in value)
                {
                    var src = source;
                    src.DataManager = DataManager;
                    src.CoreInitialize(() => CloseSource(src));
                }
                _sources = value;
            }
        }

        public IDataManager DataManager { get; set; }
        public new bool Activated { get; private set; }
        public MediaPanelBase SourceMain { get; private set; }
        public MediaPanelBase SourceSecondary { get; private set; }

        private IReadOnlyCollection<MediaPanelBase> _sources;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            Hide();
        }

        public bool IsSourceActive(MediaPanelBase source) => SourceMain == source || SourceSecondary == source;

        public void ActivateSource(MediaPanelBase sourceMain, MediaPanelBase sourceSecondary = null)
        {
            if (DataManager == null)
            {
                throw new ArgumentNullException("DataManager должен быть установлен");
            }

            if (sourceMain == null)
            {
                throw new ArgumentNullException();
            }

            if (sourceMain == sourceSecondary)
            {
                sourceSecondary = null;
            }

            if (sourceMain == SourceMain && sourceSecondary == SourceSecondary)
            {
                return;
            }

            if (sourceMain != null && sourceSecondary != null)
            {
                if (!sourceMain.IsCompatibleWith(sourceSecondary))
                {
                    sourceMain = sourceSecondary;
                    sourceSecondary = null;
                }
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Visibility != Visibility.Visible)
                {
                    Show();
                }

                if (IsSourceActive(sourceMain))
                {
                    sourceMain.Visibility = Visibility.Collapsed;
                }

                if (sourceSecondary != null && IsSourceActive(sourceSecondary))
                {
                    sourceSecondary.Visibility = Visibility.Collapsed;
                }

                SourceMain = sourceMain;
                SourceSecondary = sourceSecondary;

                foreach (var source in Sources)
                {
                    if (source != sourceMain && source != sourceSecondary)
                    {
                        CloseSourceInternal(source);
                    }
                }

                SourceMain.HorizontalAlignment = HorizontalAlignment.Left;
                SourceMain.VerticalAlignment = VerticalAlignment.Top;

                if (SourceSecondary != null)
                {
                    SourceMain.Margin = new Thickness(0, ActualHeight / 2 - ActualHeight / 4, 0, 0);

                    SourceSecondary.HorizontalAlignment = HorizontalAlignment.Left;
                    SourceSecondary.VerticalAlignment = VerticalAlignment.Top;
                    SourceSecondary.Margin = new Thickness(ActualWidth / 2, ActualHeight / 2 - ActualHeight / 4, 0, 0);
                }
                else
                {
                    SourceMain.Margin = new Thickness(0);
                }

                if (SourceMain != null)
                {
                    AppendSource(SourceMain);
                }

                if (SourceSecondary != null)
                {
                    AppendSource(SourceSecondary);
                }

                if (SourceSecondary != null)
                {
                    SourceMain.Initialize((int)(ActualWidth / 2), (int)(ActualHeight / 2));
                    SourceSecondary.Initialize((int)(ActualWidth / 2), (int)(ActualHeight / 2));
                }
                else
                {
                    SourceMain.Initialize((int)ActualWidth, (int)ActualHeight);
                }

                Activated = true;

                SourcesChanged?.Invoke(this, new EventsArgs<object>(this));
            }));

            SetWindowPos(btWfMinimize.Handle, IntPtr.Zero, 0, 0, 0, 0, 3);
        }

        private Timer _timer;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsVisibleProperty && IsVisible)
            {
                var screen = WpfScreen.GetScreenFrom(this);
                var size = new Size(screen.DeviceBounds.Width, screen.DeviceBounds.Height);
                var normalSize = Bases.Utils.TransformFromPixels(this, size);
                Width = normalSize.Width;
                Height = normalSize.Height;

                Activate();

                var thisHandle = new WindowInteropHelper(this).Handle;

                // Иногда панель задач при запуске находится
                // на переднем плане, активация окна через несколько секунд
                // переводит окно на передний план
                // Extra crutch
                var counter = 0;
                _timer = new Timer(
                    (s) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (IsVisible)
                            {
                                // Активируем панель задач
                                SetForegroundWindow(FindWindow("Shell_TrayWnd", ""));
                                Thread.Sleep(100);
                                // После небольшой паузы активируем наше окно. Зато это работает.
                                SetForegroundWindow(thisHandle);
                                Thread.Sleep(100);
                                // Затем выводим на передний план кнопку сворачивания
                                SetWindowPos(btWfMinimize.Handle, IntPtr.Zero, 0, 0, 0, 0, 3);
                            }
                        }));
                        counter++;
                        if (counter == 3)
                        {
                            _timer.Change(Timeout.Infinite, Timeout.Infinite);
                        }
                    },
                    null, 2000, 2000);
            }
        }

        public void ActivateSourceAsQuery(MediaPanelBase source)
        {
            if (SourceMain == null)
            {
                ActivateSource(source);
            }
            else if (SourceMain != null && SourceSecondary == null)
            {
                ActivateSource(SourceMain, source);
            }
            else if (SourceMain != null && SourceSecondary != null)
            {
                ActivateSource(SourceSecondary, source);
            }
        }

        public new void Hide()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                base.Hide();
                if (SourceMain != null)
                {
                    CloseSourceInternal(SourceMain);
                }

                if (SourceSecondary != null)
                {
                    CloseSourceInternal(SourceSecondary);
                }

                SourceMain = null;
                SourceSecondary = null;
                SourcesChanged?.Invoke(this, new EventsArgs<object>(this));
            }));
        }

        private void CloseSourceInternal(MediaPanelBase @base)
        {
            @base.Close();
            @base.Visibility = Visibility.Collapsed;
        }

        public void CloseSource(MediaPanelBase @base)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (SourceMain == @base && SourceSecondary != null)
                {
                    ActivateSource(SourceSecondary);
                }
                else if (SourceSecondary == @base && SourceMain != null)
                {
                    ActivateSource(SourceMain);
                }
                else if (IsSourceActive(@base))
                {
                    Hide();
                }
            }));
        }

        private void AppendSource(MediaPanelBase @base)
        {
            @base.Visibility = Visibility.Visible;
            if (!grid.Children.Contains(@base))
            {
                grid.Children.Add(@base);
            }
        }

        public event EventsHandler<object> SourcesChanged;

        private void LblMinimize_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void LblMinimize_MouseLeave(object sender, EventArgs e)
        {
            btWfMinimize.Height = 1;
            btWfMinimize.Width = 1;
            btWfMinimize.Background = Brushes.Black;
            lblMinimize.ForeColor = System.Drawing.Color.Black;
        }

        private void LblMinimize_MouseEnter(object sender, EventArgs e)
        {
            btWfMinimize.Height = 30;
            btWfMinimize.Width = 80;
            btWfMinimize.Background = Brushes.DarkSlateBlue;
            lblMinimize.ForeColor = System.Drawing.Color.White;
        }
    }
}