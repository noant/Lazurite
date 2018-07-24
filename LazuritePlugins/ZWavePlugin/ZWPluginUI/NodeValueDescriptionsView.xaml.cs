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
    public partial class NodeValueDescriptionsView : Grid
    {
        public NodeValueDescriptionsView()
        {
            InitializeComponent();
        }

        private NodeValue _nodeValue;

        public void Refresh()
        {
            RefreshWith(_nodeValue);
        }

        public void RefreshWith(NodeValue nodeValue)
        {
            stackPanel.Children.Clear();
            _nodeValue = nodeValue;
            AddDescription("Контроллер", _nodeValue.Node.Controller.Path + " (HomeID=" + _nodeValue.Node.Controller.HomeID + ")");
            AddDescription("Узел", string.Format("{0}\r\nID={1}", _nodeValue.Node.FullName, _nodeValue.Node.Id));
            AddDescription("Параметр", string.Format("{0}\r\nID={1}\r\nIndex={2}", _nodeValue.Name, _nodeValue.Id, _nodeValue.Index));
            AddDescription("Описание", _nodeValue.Description);
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
                        changeRangeButton.Margin = new Thickness(0,0,0,1);
                        changeRangeButton.Icon = LazuriteUI.Icons.Icon.MathPlusMinus;
                        changeRangeButton.Background = LazuriteUI.Windows.Controls.Visual.BrightItemBackground;
                        changeRangeButton.Content = "Изменить параметры";
                        changeRangeButton.Click += (o, e) => {
                            ChangeRangeView.Show(
                                _nodeValue.Min, 
                                _nodeValue.Max,
                                _nodeValue.Unit,
                                (min, max, unit) =>
                                {
                                    _nodeValue.Min = min;
                                    _nodeValue.Max = max;
                                    _nodeValue.Unit = unit;
                                    RefreshWith(_nodeValue);
                                },
                                Window.GetWindow(this).Content as Grid);
                        };
                        AddControl(changeRangeButton);
                    }
                    break;
                case OpenZWrapper.ValueType.List:
                    AddDescription("Возможные значения", _nodeValue.PossibleValues.Aggregate((x1, x2) => x1 + ",\r\n" + x2));
                    break;
            }
        }

        private void AddDescription(string caption, string description)
        {
            AddControl(new DescriptionItemView(caption, description));
        }

        private void AddControl(FrameworkElement element)
        {
            stackPanel.Children.Add(element);
        }
    }
}
