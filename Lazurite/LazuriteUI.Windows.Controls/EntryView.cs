using Lazurite.MainDomain;
using Lazurite.Shared;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazuriteUI.Windows.Controls
{
    public class EntryView: TextBox
    {
        public Action<EntryViewValidation> Validation;

        public event EventsHandler<EntryView> ErrorStateChanged;

        public string ErrorMessage { get; private set; }

        public bool InputWrong { get; private set; }

        private string _oldText;

        public EntryView()
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            this.Foreground = new SolidColorBrush(Colors.White);
            this.BorderThickness = new System.Windows.Thickness(0, 0, 0, 2);
            this.BorderBrush = new SolidColorBrush(Colors.SteelBlue);
            this.CaretBrush = new SolidColorBrush(Colors.SteelBlue);
            var ignoreTextChangedFlag = false;
            base.TextChanged += (o, e) => {
                if (ignoreTextChangedFlag)
                    return;
                int caretIndex = this.CaretIndex;
                var text = this.Text;
                var validation = new EntryViewValidation(this);
                validation.InputString = text;
                try
                {
                    Validation?.Invoke(validation);
                    var oldErrorState = this.ErrorMessage;
                    if (!string.IsNullOrEmpty(validation.ErrorMessage))
                    {
                        ErrorMessage = validation.ErrorMessage;
                        InputWrong = true;
                    }
                    else
                    {
                        ErrorMessage = null;
                        InputWrong = false;
                    }

                    if (oldErrorState != this.ErrorMessage)
                        ErrorStateChanged?.Invoke(this, new EventsArgs<EntryView>(this));

                    if (!string.IsNullOrEmpty(validation.OutputString))
                    {
                        ignoreTextChangedFlag = true;
                        Text = validation.OutputString;
                        ignoreTextChangedFlag = false;
                        CaretIndex = caretIndex;
                    }
                    _oldText = Text;
                    TextChanged?.Invoke(this, new RoutedEventArgs());
                }
                catch
                {
                    ignoreTextChangedFlag = true;
                    Text = _oldText;
                    ignoreTextChangedFlag = false;
                    CaretIndex = caretIndex;
                }
                if (validation.SelectAll)
                    this.SelectAll();
                validation.AfterValidation?.Invoke(this);
            };
        }

        public new event RoutedEventHandler TextChanged;
    }

    public class EntryViewValidation
    {
        public EntryViewValidation(EntryView view)
        {
            EntryView = view;
        }

        public string ErrorMessage { get; set; } = null;
        public string InputString { get; set; }
        public string OutputString { get; set; } = null;
        public bool SelectAll { get; set; } = false;
        public EntryView EntryView { get; private set; }
        public Action<EntryView> AfterValidation { get; set; }

        public static Action<EntryViewValidation> IntValidation(string fieldDescription = "", int min = int.MinValue, int max = int.MaxValue, bool roundToMargin = true)
        {
            return (v) => {
                if (v.InputString == string.Empty)
                {
                    v.OutputString = min.ToString();
                    v.SelectAll = true;
                }
                if (v.InputString == "-")
                {
                    v.InputString = "-1";
                    v.AfterValidation = (s) => v.EntryView.Select(1, 1);
                }
                var value = int.Parse(v.InputString);
                if (roundToMargin)
                {
                    if (value > max)
                        value = max;
                    else if (value < min)
                        value = min;
                }
                else
                {
                    if (value > max)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть меньше " + max;
                    else if (value < min)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть больше " + min;
                }
                v.OutputString = value.ToString();
            };
        }

        public static Action<EntryViewValidation> UIntValidation(string fieldDescription = "", uint min = uint.MinValue, uint max = uint.MaxValue, bool roundToMargin = true)
        {
            return (v) => {
                if (v.InputString == string.Empty)
                {
                    v.OutputString = min.ToString();
                    v.SelectAll = true;
                }
                if (v.InputString == "-")
                {
                    v.InputString = "-1";
                    v.AfterValidation = (s) => v.EntryView.Select(1, 1);
                }
                var value = uint.Parse(v.InputString);
                if (roundToMargin)
                {
                    if (value > max)
                        value = max;
                    else if (value < min)
                        value = min;
                }
                else
                {
                    if (value > max)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть меньше " + max;
                    else if (value < min)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть больше " + min;
                }
                v.OutputString = value.ToString();
            };
        }

        public static Action<EntryViewValidation> UShortValidation(string fieldDescription = "", ushort min = ushort.MinValue, ushort max = ushort.MaxValue, bool roundToMargin = true)
        {
            return (v) => {
                if (v.InputString == string.Empty)
                {
                    v.OutputString = min.ToString();
                    v.SelectAll = true;
                }
                if (v.InputString == "-")
                {
                    v.InputString = "-1";
                    v.AfterValidation = (s) => v.EntryView.Select(1, 1);
                }
                var value = ushort.Parse(v.InputString);
                if (roundToMargin)
                {
                    if (value > max)
                        value = max;
                    else if (value < min)
                        value = min;
                }
                else
                {
                    if (value > max)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть меньше " + max;
                    else if (value < min)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть больше " + min;
                }
                v.OutputString = value.ToString();
            };
        }

        public static Action<EntryViewValidation> ShortValidation(string fieldDescription = "", short min = short.MinValue, short max = short.MaxValue, bool roundToMargin = true)
        {
            return (v) => {
                if (v.InputString == string.Empty)
                {
                    v.OutputString = min.ToString();
                    v.SelectAll = true;
                }
                if (v.InputString == "-")
                {
                    v.InputString = "-1";
                    v.AfterValidation = (s) => v.EntryView.Select(1, 1);
                }
                var value = short.Parse(v.InputString);
                if (roundToMargin)
                {
                    if (value > max)
                        value = max;
                    else if (value < min)
                        value = min;
                }
                else
                {
                    if (value > max)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть меньше " + max;
                    else if (value < min)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть больше " + min;
                }
                v.OutputString = value.ToString();
            };
        }

        public static Action<EntryViewValidation> DoubleValidation(string fieldDescription = "", double min = double.MinValue, double max = double.MaxValue, bool roundToMargin = true)
        {
            return (v) => {
                if (v.InputString == string.Empty)
                {
                    v.OutputString = min.ToString();
                    v.SelectAll = true;
                }
                if (v.InputString == "-")
                {
                    v.InputString = "-1";
                    v.AfterValidation = (s) => v.EntryView.Select(1, 1);
                }
                var value = double.Parse(v.InputString);
                if (roundToMargin)
                {
                    if (value > max)
                        value = max;
                    else if (value < min)
                        value = min;
                }
                else
                {
                    if (value > max)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть меньше " + max;
                    else if (value < min)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть больше " + min;
                }
                v.OutputString = value.ToString();
            };
        }

        public static Action<EntryViewValidation> FloatValidation(string fieldDescription = "", float min = float.MinValue, float max = float.MaxValue, bool roundToMargin = true)
        {
            return (v) => {
                if (v.InputString == string.Empty)
                {
                    v.OutputString = min.ToString();
                    v.SelectAll = true;
                }
                if (v.InputString == "-")
                {
                    v.InputString = "-1";
                    v.AfterValidation = (s) => v.EntryView.Select(1, 1);
                }
                var value = float.Parse(v.InputString);
                if (roundToMargin)
                {
                    if (value > max)
                        value = max;
                    else if (value < min)
                        value = min;
                }
                else
                {
                    if (value > max)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть меньше " + max;
                    else if (value < min)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть больше " + min;
                }
                v.OutputString = value.ToString();
            };
        }

        public static Action<EntryViewValidation> LongValidation(string fieldDescription = "", long min = long.MinValue, long max = long.MaxValue, bool roundToMargin = true)
        {
            return (v) => {
                if (v.InputString == string.Empty)
                {
                    v.OutputString = min.ToString();
                    v.SelectAll = true;
                }
                if (v.InputString == "-")
                {
                    v.InputString = "-1";
                    v.AfterValidation = (s) => v.EntryView.Select(1, 1);
                }
                var value = long.Parse(v.InputString);
                if (roundToMargin)
                {
                    if (value > max)
                        value = max;
                    else if (value < min)
                        value = min;
                }
                else
                {
                    if (value > max)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть меньше " + max;
                    else if (value < min)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть больше " + min;
                }
                v.OutputString = value.ToString();
            };
        }

        public static Action<EntryViewValidation> ULongValidation(string fieldDescription = "", ulong min = ulong.MinValue, ulong max = ulong.MaxValue, bool roundToMargin = true)
        {
            return (v) => {
                if (v.InputString == string.Empty)
                {
                    v.OutputString = min.ToString();
                    v.SelectAll = true;
                }
                if (v.InputString == "-")
                {
                    v.InputString = "-1";
                    v.AfterValidation = (s) => v.EntryView.Select(1, 1);
                }
                var value = ulong.Parse(v.InputString);
                if (roundToMargin)
                {
                    if (value > max)
                        value = max;
                    else if (value < min)
                        value = min;
                }
                else
                {
                    if (value > max)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть меньше " + max;
                    else if (value < min)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть больше " + min;
                }
                v.OutputString = value.ToString();
            };
        }

        public static Action<EntryViewValidation> DecimalValidation(string fieldDescription = "", decimal min = decimal.MinValue, decimal max = decimal.MaxValue, bool roundToMargin = true)
        {
            return (v) => {
                if (v.InputString == string.Empty)
                {
                    v.OutputString = min.ToString();
                    v.SelectAll = true;
                }
                if (v.InputString == "-")
                {
                    v.InputString = "-1";
                    v.AfterValidation = (s) => v.EntryView.Select(1, 1);
                }
                var value = decimal.Parse(v.InputString);
                if (roundToMargin)
                {
                    if (value > max)
                        value = max;
                    else if (value < min)
                        value = min;
                }
                else
                {
                    if (value > max)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть меньше " + max;
                    else if (value < min)
                        v.ErrorMessage = "Значение поля [" + fieldDescription + "] должно быть больше " + min;
                }
                v.OutputString = value.ToString();
            };
        }
    }
}
