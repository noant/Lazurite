using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
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
