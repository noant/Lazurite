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

            btApply.Click += (o, e) => ApplyClick?.Invoke();            
            btCancel.Click += (o, e) => CancelClick?.Invoke();
        }

        public event Action ApplyClick;
        public event Action CancelClick;
    }
}
