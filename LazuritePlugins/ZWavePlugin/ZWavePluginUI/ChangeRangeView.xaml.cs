using LazuriteUI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

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
            this.tbMin.Validation = EntryViewValidation.DecimalValidation(max: decimal.Parse(this.tbMax.Text));
            this.tbMax.Validation = EntryViewValidation.DecimalValidation(min: decimal.Parse(this.tbMin.Text));
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
