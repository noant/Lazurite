using Lazurite.Shared;
using System;
using System.Windows;

namespace LazuriteUI.Windows.Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class RequireAdminRightsWindow : Window
    {
        public RequireAdminRightsWindow()
        {
            InitializeComponent();

            btApply.Click += (o, e) =>
            {
                btCancel.IsEnabled = btApply.IsEnabled = false;
                ApplyClick?.Invoke(this, new EventsArgs<object>(this));
            };
            btCancel.Click += (o, e) => CancelClick?.Invoke(this, new EventsArgs<object>(this));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventsHandler<object> ApplyClick;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventsHandler<object> CancelClick;
    }
}
