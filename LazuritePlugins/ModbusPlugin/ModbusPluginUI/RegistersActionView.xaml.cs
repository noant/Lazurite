using Lazurite.ActionsDomain.ValueTypes;
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
    /// Логика взаимодействия для SingleCoilActionView.xaml
    /// </summary>
    public partial class RegistersActionView : UserControl
    {
        public RegistersActionView()
        {
            InitializeComponent();
            tbAddress.Validation = EntryViewValidation.UShortValidation(max: 247);
            tbCoil.Validation = EntryViewValidation.UShortValidation();
            
            btOk.Click += (o, e) => {
                _action.Manager.Transport = transportView.Transport;
                _action.RegisterAddress = byte.Parse(tbCoil.Text);
                _action.SlaveAddress = byte.Parse(tbAddress.Text);
                _action.ModbusValueType = valueTypeView.ValueType;
                _action.WriteReadLength = valueTypeView.Length;
                _action.RegistersMode = _selectedRegistersMode;
                if (_action.ModbusValueType == NModbusWrapper.ValueType.String)
                    _action.ValueType = new InfoValueType();
                else
                    _action.ValueType = new FloatValueType() {
                        AcceptedValues = new[] { tbMin.Text, tbMax.Text }
                    };
                OkPressed?.Invoke(_action);
            };

            btRegistersModeHolding.Click += (o, e) => _selectedRegistersMode = RegistersMode.Holding;
            btRegistersModeInput.Click += (o, e) => _selectedRegistersMode = RegistersMode.Input;

            btCancel.Click += (o, e) => CancelPressed?.Invoke();
        }

        private RegistersMode _selectedRegistersMode;
        private IModbusRegistersAction _action;
        private ValueTypeMode _mode;

        public void RefreshWith(IModbusRegistersAction action, ValueTypeMode mode)
        {
            _mode = mode;
            _action = action;
            
            transportView.RefreshWith(action.Manager.Transport);

            if (mode == ValueTypeMode.All)
            {
                valueTypeView.RefreshWith(action.ModbusValueType, action.WriteReadLength, mode);
            }
            else if (mode == ValueTypeMode.String && action.ModbusValueType != NModbusWrapper.ValueType.String)
            {
                valueTypeView.RefreshWith(NModbusWrapper.ValueType.String, action.WriteReadLength, mode);
            }
            else if (mode == ValueTypeMode.Numeric && action.ModbusValueType == NModbusWrapper.ValueType.String)
            {
                valueTypeView.RefreshWith(NModbusWrapper.ValueType.Double, action.WriteReadLength, mode);
            }
            else
            {
                valueTypeView.RefreshWith(action.ModbusValueType, action.WriteReadLength, mode);
            }

            tbAddress.Text = action.SlaveAddress.ToString();
            tbCoil.Text = action.RegisterAddress.ToString();

            _selectedRegistersMode = action.RegistersMode;
            if (_selectedRegistersMode == RegistersMode.Holding)
                btRegistersModeHolding.Selected = true;
            else btRegistersModeInput.Selected = true;

            ReInitializeMaxAndMin();
        }

        private void ReInitializeMaxAndMin()
        {
            if (valueTypeView.ValueType != NModbusWrapper.ValueType.String && _mode == ValueTypeMode.All)
            {
                rowMax.Height =
                    rowMin.Height = new GridLength();
                tbMin.Text = _action.ValueType.AcceptedValues[0];
                tbMax.Text = _action.ValueType.AcceptedValues[1];
            }
            else
            {
                rowMax.Height =
                    rowMin.Height = new GridLength(0);
            }
            ReInitializeMaxAndMinValidation();
        }

        private void ReInitializeMaxAndMinValidation()
        {
            switch (valueTypeView.ValueType)
            {
                case NModbusWrapper.ValueType.Double:
                    {
                        tbMin.Validation = (o, e) => EntryViewValidation.DoubleValidation(max: double.Parse(tbMax.Text)).Invoke(o, e);
                        tbMax.Validation = (o, e) => EntryViewValidation.DoubleValidation(min: double.Parse(tbMin.Text)).Invoke(o, e);
                        break;
                    }
                case NModbusWrapper.ValueType.Float:
                    {
                        tbMin.Validation = (o, e) => EntryViewValidation.FloatValidation(max: float.Parse(tbMax.Text)).Invoke(o, e);
                        tbMax.Validation = (o, e) => EntryViewValidation.FloatValidation(min: float.Parse(tbMin.Text)).Invoke(o, e);
                        break;
                    }
                case NModbusWrapper.ValueType.Int:
                    {
                        tbMin.Validation = (o, e) => EntryViewValidation.IntValidation(max: int.Parse(tbMax.Text)).Invoke(o, e);
                        tbMax.Validation = (o, e) => EntryViewValidation.IntValidation(min: int.Parse(tbMin.Text)).Invoke(o, e);
                        break;
                    }
                case NModbusWrapper.ValueType.Long:
                    {
                        tbMin.Validation = (o, e) => EntryViewValidation.LongValidation(max: long.Parse(tbMax.Text)).Invoke(o, e);
                        tbMax.Validation = (o, e) => EntryViewValidation.LongValidation(min: long.Parse(tbMin.Text)).Invoke(o, e);
                        break;
                    }
                case NModbusWrapper.ValueType.Short:
                    {
                        tbMin.Validation = (o, e) => EntryViewValidation.LongValidation(max: short.Parse(tbMax.Text)).Invoke(o, e);
                        tbMax.Validation = (o, e) => EntryViewValidation.LongValidation(min: short.Parse(tbMin.Text)).Invoke(o, e);
                        break;
                    }
                case NModbusWrapper.ValueType.UInt:
                    {
                        tbMin.Validation = (o, e) => EntryViewValidation.UIntValidation(max: uint.Parse(tbMax.Text)).Invoke(o, e);
                        tbMax.Validation = (o, e) => EntryViewValidation.UIntValidation(min: uint.Parse(tbMin.Text)).Invoke(o, e);
                        break;
                    }
                case NModbusWrapper.ValueType.ULong:
                    {
                        tbMin.Validation = (o, e) => EntryViewValidation.ULongValidation(max: ulong.Parse(tbMax.Text)).Invoke(o, e);
                        tbMax.Validation = (o, e) => EntryViewValidation.ULongValidation(min: ulong.Parse(tbMin.Text)).Invoke(o, e);
                        break;
                    }
                case NModbusWrapper.ValueType.UShort:
                    {
                        tbMin.Validation = (o, e) => EntryViewValidation.UShortValidation(max: ushort.Parse(tbMax.Text)).Invoke(o, e);
                        tbMax.Validation = (o, e) => EntryViewValidation.UShortValidation(min: ushort.Parse(tbMin.Text)).Invoke(o, e);
                        break;
                    }
                case NModbusWrapper.ValueType.String:
                    {
                        tbMin.Validation = null;
                        tbMax.Validation = null;
                        break;
                    }
                default: throw new Exception("Type not exist");
            }
        }

        public Action<IModbusRegistersAction> OkPressed;
        public Action CancelPressed;
    }
}
