using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Common
{
    public partial class CheckPanel : Grid
    {
        public static BindableProperty CheckedProperty;

        static CheckPanel()
        {
            CheckedProperty = BindableProperty.Create(nameof(Checked), typeof(bool), typeof(CheckPanel), false, BindingMode.Default, null,
                (o, oldVal, newVal) =>
                {
                    var checkPanel = o as CheckPanel;
                    var backColor = (bool)newVal ? Color.FromHex("#8b008b") : Color.Gray;
                    checkPanel.bv1.Color =
                        checkPanel.bv2.Color =
                        checkPanel.bv3.Color =
                        checkPanel.bv4.Color = backColor;
                }
            );
        }

        public CheckPanel()
        {
            InitializeComponent();
        }

        public bool Checked
        {
            get
            {
                return (bool)GetValue(CheckedProperty);
            }
            set
            {
                SetValue(CheckedProperty, value);
            }
        }
    }
}
