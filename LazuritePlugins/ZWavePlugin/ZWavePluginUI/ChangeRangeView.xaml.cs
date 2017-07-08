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
            this.tbMin.Validation = (str) => decimal.Parse(str) < decimal.Parse(this.tbMax.Text);
            this.tbMax.Validation = (str) => decimal.Parse(str) > decimal.Parse(this.tbMin.Text);
        }

        public decimal Max
        {
            get
            {
                return decimal.Parse(tbMax.Text);
            }
            set
            {
                tbMax.Text = value.ToString();
            }
        }

        public decimal Min
        {
            get
            {
                return decimal.Parse(tbMin.Text);
            }
            set
            {
                tbMin.Text = value.ToString();
            }
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            OkClicked?.Invoke(this);
        }

        public event Action<ChangeRangeView> OkClicked;
    }
}
