using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Windows.Controls;
using NModbusWrapper;
using System;
using System.Windows;
using System.Windows.Controls;

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

            valueTypeView.ValueTypeChanged += (o, e) => ReInitializeMaxAndMin();            

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
            rowMax.Height = rowMin.Height = new GridLength(0);
            if (valueTypeView.ValueType != NModbusWrapper.ValueType.String && _mode != ValueTypeMode.String)
                rowMax.Height = rowMin.Height = new GridLength();
            ReInitializeMaxAndMinValidation();
        }

        private void ReInitializeMaxAndMinValidation()
        {
            //crutch
            tbMin.BeginChange();
            tbMax.BeginChange();
            tbMin.Text = tbMax.Text = "0";
            tbMax.EndChange();
            tbMin.EndChange();
            tbMin.Validation = tbMax.Validation = null;


            switch (valueTypeView.ValueType)
            {
                case NModbusWrapper.ValueType.Double:
                    {
                        tbMin.Text = double.MinValue.ToString();
                        tbMax.Text = double.MaxValue.ToString();
                        tbMin.Validation = (e) => EntryViewValidation.DoubleValidation(max: double.Parse(tbMax.Text)).Invoke(e);
                        tbMax.Validation = (e) => EntryViewValidation.DoubleValidation(min: double.Parse(tbMin.Text)).Invoke(e);
                        break;
                    }
                case NModbusWrapper.ValueType.Float:
                    {
                        tbMin.Text = float.MinValue.ToString();
                        tbMax.Text = float.MaxValue.ToString();
                        tbMin.Validation = (e) => EntryViewValidation.FloatValidation(max: float.Parse(tbMax.Text)).Invoke(e);
                        tbMax.Validation = (e) => EntryViewValidation.FloatValidation(min: float.Parse(tbMin.Text)).Invoke(e);
                        break;
                    }
                case NModbusWrapper.ValueType.Int:
                    {
                        tbMin.Text = int.MinValue.ToString();
                        tbMax.Text = int.MaxValue.ToString();
                        tbMin.Validation = (e) => EntryViewValidation.IntValidation(max: int.Parse(tbMax.Text)).Invoke(e);
                        tbMax.Validation = (e) => EntryViewValidation.IntValidation(min: int.Parse(tbMin.Text)).Invoke(e);
                        break;
                    }
                case NModbusWrapper.ValueType.Long:
                    {
                        tbMin.Text = long.MinValue.ToString();
                        tbMax.Text = long.MaxValue.ToString();
                        tbMin.Validation = (e) => EntryViewValidation.LongValidation(max: long.Parse(tbMax.Text)).Invoke(e);
                        tbMax.Validation = (e) => EntryViewValidation.LongValidation(min: long.Parse(tbMin.Text)).Invoke(e);
                        break;
                    }
                case NModbusWrapper.ValueType.Short:
                    {
                        tbMin.Text = short.MinValue.ToString();
                        tbMax.Text = short.MaxValue.ToString();
                        tbMin.Validation = (e) => EntryViewValidation.LongValidation(max: short.Parse(tbMax.Text)).Invoke(e);
                        tbMax.Validation = (e) => EntryViewValidation.LongValidation(min: short.Parse(tbMin.Text)).Invoke(e);
                        break;
                    }
                case NModbusWrapper.ValueType.UInt:
                    {
                        tbMin.Text = uint.MinValue.ToString();
                        tbMax.Text = uint.MaxValue.ToString();
                        tbMin.Validation = (e) => EntryViewValidation.UIntValidation(max: uint.Parse(tbMax.Text)).Invoke(e);
                        tbMax.Validation = (e) => EntryViewValidation.UIntValidation(min: uint.Parse(tbMin.Text)).Invoke(e);
                        break;
                    }
                case NModbusWrapper.ValueType.ULong:
                    {
                        tbMin.Text = ulong.MinValue.ToString();
                        tbMax.Text = ulong.MaxValue.ToString();
                        tbMin.Validation = (e) => EntryViewValidation.ULongValidation(max: ulong.Parse(tbMax.Text)).Invoke(e);
                        tbMax.Validation = (e) => EntryViewValidation.ULongValidation(min: ulong.Parse(tbMin.Text)).Invoke(e);
                        break;
                    }
                case NModbusWrapper.ValueType.UShort:
                    {
                        tbMin.Text = ushort.MinValue.ToString();
                        tbMax.Text = ushort.MaxValue.ToString();
                        tbMin.Validation = (e) => EntryViewValidation.UShortValidation(max: ushort.Parse(tbMax.Text)).Invoke(e);
                        tbMax.Validation = (e) => EntryViewValidation.UShortValidation(min: ushort.Parse(tbMin.Text)).Invoke(e);
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
