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

namespace ZWavePluginUI
{
    /// <summary>
    /// Логика взаимодействия для ChangeRangeView.xaml
    /// </summary>
    public partial class ChangeRangeView : UserControl
    {
        public ChangeRangeView()
        {
            InitializeComponent();
            this.nudMax.MaxValue = this.nudMin.MaxValue = decimal.MaxValue;
            this.nudMax.MinValue = this.nudMin.MinValue = decimal.MinValue;
        }

        public decimal Max
        {
            get
            {
                return nudMax.Value;
            }
            set
            {
                nudMax.Value = value;
            }
        }

        public decimal Min
        {
            get
            {
                return nudMin.Value;
            }
            set
            {
                nudMin.Value = value;
            }
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            OkClicked?.Invoke(this);
        }

        public event Action<ChangeRangeView> OkClicked;
    }
}
