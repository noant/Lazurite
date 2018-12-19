using Lazurite.Shared;
using MediaHost.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaHost.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IReadOnlyCollection<MediaPanelBase> Sources
        {
            get => _sources;
            set
            {
                if (DataManager == null)
                    throw new ArgumentNullException("DataManager должен быть установлен");

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
                throw new ArgumentNullException("DataManager должен быть установлен");

            if (sourceMain == null)
                throw new ArgumentNullException();
            
            if (sourceMain == sourceSecondary)
                sourceSecondary = null;

            if (sourceMain == SourceMain && sourceSecondary == SourceSecondary)
                return;
            
            if (sourceMain != null && sourceSecondary != null)
                if (!sourceMain.IsCompatibleWith(sourceSecondary))
                {
                    sourceMain = sourceSecondary;
                    sourceSecondary = null;
                }

            Dispatcher.BeginInvoke(new Action(() => {
                if (Visibility != Visibility.Visible)
                {
                    Show();
                    Activate();
                }

                if (IsSourceActive(sourceMain))
                    sourceMain.Visibility = Visibility.Collapsed;

                if (sourceSecondary != null && IsSourceActive(sourceSecondary))
                    sourceSecondary.Visibility = Visibility.Collapsed;

                SourceMain = sourceMain;
                SourceSecondary = sourceSecondary;

                foreach (var source in Sources)
                    if (source != sourceMain && source != sourceSecondary)
                        CloseSourceInternal(source);

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
                    SourceMain.Margin = new Thickness(0);
                
                if (SourceMain != null)
                    AppendSource(SourceMain);
                if (SourceSecondary != null)
                    AppendSource(SourceSecondary);

                if (SourceSecondary != null)
                {
                    SourceMain.Initialize((int)(ActualWidth / 2), (int)(ActualHeight / 2));
                    SourceSecondary.Initialize((int)(ActualWidth / 2), (int)(ActualHeight / 2));
                }
                else
                    SourceMain.Initialize((int)ActualWidth, (int)ActualHeight);

                Activated = true;

                SourcesChanged?.Invoke(this, new EventsArgs<object>(this));
            }));
        }

        public void ActivateSourceAsQuery(MediaPanelBase source)
        {
            if (SourceMain == null)
                ActivateSource(source);
            else if (SourceMain != null && SourceSecondary == null)
                ActivateSource(SourceMain, source);
            else if (SourceMain != null && SourceSecondary != null)
                ActivateSource(SourceSecondary, source);
        }

        public new void Hide()
        {
            Dispatcher.BeginInvoke(new Action(() => {
                base.Hide();
                if (SourceMain != null)
                    CloseSourceInternal(SourceMain);
                if (SourceSecondary != null)
                    CloseSourceInternal(SourceSecondary);
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
                    ActivateSource(SourceSecondary);
                else if (SourceSecondary == @base && SourceMain != null)
                    ActivateSource(SourceMain);
                else if (IsSourceActive(@base))
                    Hide();
            }));
        }

        private void AppendSource(MediaPanelBase @base)
        {
            @base.Visibility = Visibility.Visible;
            if (!grid.Children.Contains(@base))
                grid.Children.Add(@base);
        }

        public event EventsHandler<object> SourcesChanged;
    }
}
