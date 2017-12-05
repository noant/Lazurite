using LazuriteUI.Windows.Controls;
using System.Windows.Controls;

namespace WakeOnLanUI
{
    /// <summary>
    /// Логика взаимодействия для IpBaseView.xaml
    /// </summary>
    public partial class IpBaseView : UserControl
    {
        public IpBaseView()
        {
            InitializeComponent();

            tbPart1.Validation =
                tbPart2.Validation =
                tbPart3.Validation = EntryViewValidation.UShortValidation(min: 0, max: 255);
        }

        public byte[] IpBase
        {
            get
            {
                return new byte[] {
                    byte.Parse(tbPart1.Text),
                    byte.Parse(tbPart2.Text),
                    byte.Parse(tbPart3.Text)
                };
            }
        }
    }
}
