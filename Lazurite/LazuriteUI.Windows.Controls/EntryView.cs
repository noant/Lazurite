using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazuriteUI.Windows.Controls
{
    public class EntryView: TextBox
    {
        public Func<string, object> Validation;

        private string _oldText;

        public EntryView()
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            this.Foreground = new SolidColorBrush(Colors.White);
            this.BorderThickness = new System.Windows.Thickness(0, 0, 0, 2);
            this.BorderBrush = new SolidColorBrush(Colors.SteelBlue);

            base.TextChanged += (o, e) => {
                int caretIndex = this.CaretIndex;
                var text = this.Text;
                try
                {
                    var result = Validation?.Invoke(text);
                    if (result is bool && !(bool)result)
                        throw new Exception();
                    TextChanged?.Invoke(this, new RoutedEventArgs());
                    _oldText = text;
                }
                catch
                {
                    this.Text = _oldText;
                    this.CaretIndex = caretIndex;
                }
            };
        }

        public new event RoutedEventHandler TextChanged;
    }
}
