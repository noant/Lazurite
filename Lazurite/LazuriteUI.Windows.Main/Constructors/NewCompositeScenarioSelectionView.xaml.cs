using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Windows.Main.Switches;
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
