using LazuriteUI.Windows.Controls;
using OpenZWrapper;
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

namespace ZWPluginUI
{
    /// <summary>
    /// Логика взаимодействия для NodeValueDescriptionsView.xaml
    /// </summary>
    public partial class NodeValueDescriptionsView : UserControl
    {
        public NodeValueDescriptionsView()
        {
            InitializeComponent();
        }

        private NodeValue _nodeValue;

        public void RefreshWith(NodeValue nodeValue)
        {
            stackPanel.Children.Clear();
            _nodeValue = nodeValue;
            AddDescription("Контроллер", _nodeValue.Node.Controller.Path);
            AddDescription("Узел", string.Format("{0}\r\n{1}\r\nID={2}", _nodeValue.Node.Manufacturer, _nodeValue.Node.ProductName, _nodeValue.Node.Id));
            AddDescription("Параметр", string.Format("{0}\r\nID={1}", _nodeValue.Name, _nodeValue.Id));
            AddDescription("Тип параметра", Enum.GetName(typeof(OpenZWrapper.ValueType), _nodeValue.ValueType));
            if (_nodeValue.ValueType != OpenZWrapper.ValueType.Button)
                AddDescription("Текущее значение", _nodeValue.Current?.ToString());
            switch(_nodeValue.ValueType)
            {
                case OpenZWrapper.ValueType.Byte:
                case OpenZWrapper.ValueType.Decimal:
                case OpenZWrapper.ValueType.Int:
                case OpenZWrapper.ValueType.Short:
                    {
                        AddDescription("Возможные значения", string.Format("Минимум: {0}\r\nМаксимум: {1}\r\n{2}", _nodeValue.Min, _nodeValue.Max, _nodeValue.Unit));
                        var changeRangeButton = new ItemView();
                        changeRangeButton.Icon = LazuriteUI.Icons.Icon.MathPlusMinus;
                        changeRangeButton.Content = "Изменить максимум и минимум";
                        changeRangeButton.Click += (o, e) => {
                            ChangeRangeView.Show(_nodeValue.Min, _nodeValue.Max,
                                (min, max) =>
                                {
                                    _nodeValue.Min = min;
                                    _nodeValue.Max = max;
                                    RefreshWith(_nodeValue);
                                },
                                Window.GetWindow(this).Content as Grid);
                        };
                        stackPanel.Children.Add(changeRangeButton);
                    }
                    break;
                case OpenZWrapper.ValueType.List:
                    AddDescription("Возможные значения", _nodeValue.PossibleValues.Aggregate((x1, x2) => x1 + ",\r\n" + x2));
                    break;
            }
            AddDescription("Описание", _nodeValue.Description);
        }

        private void AddDescription(string caption, string description)
        {
            stackPanel.Children.Add(new DescriptionItemView(caption, description));
        }
    }
}
