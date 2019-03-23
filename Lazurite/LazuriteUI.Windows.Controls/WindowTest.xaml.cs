using System.Windows;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для WindowTest.xaml
    /// </summary>
    public partial class WindowTest : Window
    {
        public WindowTest()
        {
            InitializeComponent();

            entryView.Validation = (s) => EntryViewValidation.UIntValidation().Invoke(s);
        }
    }
}