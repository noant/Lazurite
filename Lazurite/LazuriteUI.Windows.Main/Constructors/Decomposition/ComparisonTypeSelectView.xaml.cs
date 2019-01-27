using Lazurite.CoreActions.ComparisonTypes;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ComparisonTypeSelectView.xaml
    /// </summary>
    public partial class ComparisonTypeSelectView : UserControl
    {
        public ComparisonTypeSelectView(IComparisonType comparisonType, bool onlyNumeric)
        {
            InitializeComponent();

            var comparisonTypes = Lazurite.CoreActions.Utils.GetComparisonTypes();
            if (!onlyNumeric)
                comparisonTypes = Lazurite.CoreActions.Utils.GetComparisonTypes().Where(x => !x.OnlyNumeric).ToArray();
            foreach (var type in comparisonTypes)
            {
                var cType = type;
                var itemView = new ItemView();
                if (type.GetType().Equals(comparisonType.GetType()))
                    Loaded += (o, e) => itemView.Selected = true;
                itemView.Content = type.Caption;
                itemView.Margin = new Thickness(0, 0, 0, 1);
                itemView.IconVisibility = Visibility.Collapsed;
                itemView.Click += (o, e) => Selected?.Invoke(cType);
                listItems.Children.Add(itemView);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<IComparisonType> Selected;

        public static void Show(Action<IComparisonType> callback, IComparisonType selected, bool onlyNumeric)
        {
            var control = new ComparisonTypeSelectView(selected, onlyNumeric);
            var dialog = new DialogView(control);
            dialog.ShowUnderCursor = true;
            control.Selected += (type) => {
                callback?.Invoke(type);
                dialog.Close();
            };
            dialog.Show();
        }
    }
}