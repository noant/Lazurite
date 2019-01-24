using Lazurite.Shared;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Common
{
    /// <summary>
    /// Логика взаимодействия для DatesRangeItemView.xaml
    /// </summary>
    public partial class DatesRangeItemView : Grid
    {
        public static readonly DependencyProperty MaxProperty;
        public static readonly DependencyProperty MinProperty;
        public static readonly DependencyProperty DateSelectionItemProperty;

        static DatesRangeItemView()
        {
            MaxProperty = DependencyProperty.Register(nameof(Max), typeof(DateTime), typeof(DatesRangeItemView), new PropertyMetadata(DateTime.Now.AddYears(1)));
            MinProperty = DependencyProperty.Register(nameof(Min), typeof(DateTime), typeof(DatesRangeItemView), new PropertyMetadata(DateTime.Now.AddYears(-1)));

            DateSelectionItemProperty = DependencyProperty.Register(nameof(DateSelectionItem), typeof(DateSelectionItem), typeof(DatesRangeItemView), new PropertyMetadata(new DateSelectionItem(DateSelection.LastWeek)) { PropertyChangedCallback = (o,e) => ((DatesRangeItemView)o).RaiseDateChanged() });
        }

        public DatesRangeItemView()
        {
            InitializeComponent();
        }

        public DateTime Max {
            get => (DateTime)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public DateTime Min
        {
            get => (DateTime)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public DateSelectionItem DateSelectionItem
        {
            get => (DateSelectionItem)GetValue(DateSelectionItemProperty);
            set => SetValue(DateSelectionItemProperty, value);
        }

        private void RaiseDateChanged()
        {
            switch(DateSelectionItem.DateSelection)
            {
                case DateSelection.Custom:
                    lblDateView.Content = string.Format("{0} - {1}", DateSelectionItem.Start.ToShortDateString(), DateSelectionItem.End.ToShortDateString());
                    break;
                case DateSelection.LastMonth:
                    lblDateView.Content = "Последний месяц";
                    break;
                case DateSelection.LastWeek:
                    lblDateView.Content = "Последняя неделя";
                    break;
                case DateSelection.LastYear:
                    lblDateView.Content = "Последний год";
                    break;
                case DateSelection.Last24Hours:
                    lblDateView.Content = "Последние сутки";
                    break;
            }
            SelectionChanged?.Invoke(this, new EventsArgs<DateSelectionItem>(DateSelectionItem));
        }

        public event EventsHandler<DateSelectionItem> SelectionChanged;

        private void btSelect_Click(object sender, RoutedEventArgs e)
        {
            DatesRangeSelectView.Show((item) => DateSelectionItem = item, Max, Min, DateSelectionItem);
        }
    }

}
