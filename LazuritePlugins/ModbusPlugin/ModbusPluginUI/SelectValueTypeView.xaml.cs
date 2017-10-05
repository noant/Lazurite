using NModbusWrapper;
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

namespace ModbusPluginUI
{
    /// <summary>
    /// Логика взаимодействия для SelectValueTypeView.xaml
    /// </summary>
    public partial class SelectValueTypeView : UserControl
    {
        public SelectValueTypeView()
        {
            InitializeComponent();

            this.Loaded += (o, e) => {
                var grid = Window.GetWindow(this).Content as Grid;
                btEdit.Click += (o1, e1) => {
                    ModbusValueTypeSelectionView.Show(
                        grid,
                        this.ValueType,
                        this.Length,
                        (type, length) => {
                            RefreshWith(type, length, _mode);
                            ValueTypeChanged?.Invoke(length, type);
                        },
                        _mode);
                };
            };
        }

        public void RefreshWith(NModbusWrapper.ValueType valueType, ushort length, ValueTypeMode mode)
        {
            lblDescription.Content = Enum.GetName(typeof(NModbusWrapper.ValueType), valueType);
            if (valueType == NModbusWrapper.ValueType.String)
                lblDescription.Content = lblDescription.Content.ToString() + ", " + length.ToString() + " символов";
            ValueType = valueType;
            Length = length;
            _mode = mode;
        }

        public NModbusWrapper.ValueType ValueType { get; private set; }
        public ushort Length { get; private set; }
        private ValueTypeMode _mode;

        public event Action<ushort, NModbusWrapper.ValueType> ValueTypeChanged;
    }
}
