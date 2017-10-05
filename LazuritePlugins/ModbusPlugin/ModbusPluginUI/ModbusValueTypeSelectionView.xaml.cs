using LazuriteUI.Windows.Controls;
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
    /// Логика взаимодействия для ModbusValueTypeSelectionView.xaml
    /// </summary>
    public partial class ModbusValueTypeSelectionView : UserControl
    {
        public ModbusValueTypeSelectionView(ushort length, NModbusWrapper.ValueType valueType, ValueTypeMode mode)
        {
            InitializeComponent();

            lvItems.SelectionChanged += (o, e) => {
                btOk.IsEnabled = lvItems.SelectedItem != null;
                if (lvItems.SelectedItem != null && ((ItemView)lvItems.SelectedItem).Tag.ToString().Equals("String"))
                    spStringLength.Visibility = Visibility.Visible;
                else spStringLength.Visibility = Visibility.Collapsed;
            };
            
            if (mode == ValueTypeMode.String)
            {
                lvItems.GetItems().Where(x => ((ItemView)x).Tag.ToString() != "String").ToList()
                    .ForEach(x => ((ItemView)x).IsEnabled = false);
            }
            else if (mode == ValueTypeMode.Numeric)
            {
                lvItems.GetItems().Where(x => ((ItemView)x).Tag.ToString() == "String").ToList()
                    .ForEach(x => ((ItemView)x).IsEnabled = false);
            }

            tbLength.Validation = EntryViewValidation.UShortValidation(min: 1);

            btCancel.Click += (o, e) => CancelPressed?.Invoke();
            btOk.Click += (o, e) => OkPressed?.Invoke();
        }

        public NModbusWrapper.ValueType GetValueType()
        {
            if (lvItems.SelectedItem == null)
                return NModbusWrapper.ValueType.Int;
            else
            {
                var name = ((ItemView)lvItems.SelectedItem).Tag.ToString();
                return (NModbusWrapper.ValueType)Enum.Parse(typeof(NModbusWrapper.ValueType), name);
            }
        }

        public void SetValueType(NModbusWrapper.ValueType valueType)
        {
            var name = Enum.GetName(typeof(NModbusWrapper.ValueType), valueType);
            lvItems.GetItems().FirstOrDefault(x => ((ItemView)x).Tag.ToString().Equals(name)).Selected = true;
        }

        public ushort GetValueLength()
        {
            return ushort.Parse(tbLength.Text);
        }

        public void SetValueLength(int value)
        {
            tbLength.Text = value.ToString();
        }

        public event Action OkPressed;
        public event Action CancelPressed;

        public static void Show(Grid parent, NModbusWrapper.ValueType valueType, ushort length, Action<NModbusWrapper.ValueType, ushort> callback, ValueTypeMode mode)
        {
            var control = new ModbusValueTypeSelectionView(length, valueType, mode);
            var dialog = new DialogView(control);
            control.OkPressed += () => {
                callback?.Invoke(control.GetValueType(), control.GetValueLength());
                dialog.Close();
            };
            control.SetValueType(valueType);
            control.SetValueLength(length);
            control.CancelPressed += () => dialog.Close();
            dialog.Show(parent);
        }
    }
}
