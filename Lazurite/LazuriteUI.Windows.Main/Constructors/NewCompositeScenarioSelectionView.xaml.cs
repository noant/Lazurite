using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для NewCompositeScenarioSelectionView.xaml
    /// </summary>
    public partial class NewCompositeScenarioSelectionView : UserControl
    {
        public NewCompositeScenarioSelectionView()
        {
            InitializeComponent();
            
            btButtonSelect.Click += (o, e) => Select(new ButtonValueType());
            btToggleSelect.Click += (o, e) => Select(new ToggleValueType());
            btFloatSelect.Click += (o, e) => Select(new FloatValueType());
            btDateTimeSelect.Click += (o, e) => Select(new DateTimeValueType());
            btStatusSelect.Click += (o, e) => Select(new StateValueType());
            btInfoSelect.Click += (o, e) => Select(new InfoValueType());
        }

        private void Select(ValueTypeBase type)
        {
            SelectedType = type;
            Selected?.Invoke(type);
        }

        public ValueTypeBase SelectedType { get; private set; }

        public event Action<ValueTypeBase> Selected;
    }
}
