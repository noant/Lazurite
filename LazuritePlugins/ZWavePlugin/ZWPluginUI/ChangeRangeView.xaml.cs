using LazuriteUI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ZWPluginUI
{
    /// <summary>
    /// Логика взаимодействия для ChangeRangeView.xaml
    /// </summary>
    public partial class ChangeRangeView : UserControl
    {
        public ChangeRangeView()
        {
            InitializeComponent();
            tbMin.Validation = (e) => EntryViewValidation.DecimalValidation(max: decimal.Parse(tbMax.Text)).Invoke(e);
            tbMax.Validation = (e) => EntryViewValidation.DecimalValidation(min: decimal.Parse(tbMin.Text)).Invoke(e);
        }

        public decimal Max
        {
            get => decimal.Parse(tbMax.Text);
            set => tbMax.Text = value.ToString();
        }

        public decimal Min
        {
            get => decimal.Parse(tbMin.Text);
            set => tbMin.Text = value.ToString();
        }

        public string Unit
        {
            get => tbUnit.Text;
            set => tbUnit.Text = value;
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            OkClicked?.Invoke(this);
        }

        public event Action<ChangeRangeView> OkClicked;

        public static void Show(decimal min, decimal max, string unit, Action<decimal, decimal, string> callback, Grid parent)
        {
            var changeRangeView = new ChangeRangeView();
            changeRangeView.Max = max;
            changeRangeView.Min = min;
            changeRangeView.Unit = unit;
            var dialog = new DialogView(changeRangeView);
            changeRangeView.OkClicked += (s) =>
            {
                callback?.Invoke(s.Min, s.Max, s.Unit);
                dialog.Close();
            };
            dialog.Show(parent);
        }
    }
}
