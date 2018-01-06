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
    /// Логика взаимодействия для DescriptionItemView.xaml
    /// </summary>
    public partial class DescriptionItemView : Grid
    {
        public DescriptionItemView(string caption, string description)
        {
            InitializeComponent();

            this.lblCaption.Content = caption + " >>";
            this.lblCaption.ToolTip = caption;
            this.tbText.Text = description;
            this.tbText.ToolTip = description;
            if (string.IsNullOrEmpty(description))
                this.Visibility = Visibility.Collapsed;
        }
    }
}
