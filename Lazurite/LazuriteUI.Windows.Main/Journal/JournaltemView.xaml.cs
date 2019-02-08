using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Journal
{
    /// <summary>
    /// Логика взаимодействия для JournaltemView.xaml
    /// </summary>
    public partial class JournaltemView : UserControl
    {
        public JournaltemView()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var windowDescription = new JournalItemViewWindow();
            windowDescription.DataContext = DataContext;
            windowDescription.Show();
        }
    }
}
