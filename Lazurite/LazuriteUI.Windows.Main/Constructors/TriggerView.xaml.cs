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

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для TriggerView.xaml
    /// </summary>
    public partial class TriggerView : UserControl, ITriggerConstructorView
    {
        public TriggerView(Lazurite.MainDomain.TriggerBase trigger)
        {
            InitializeComponent();
        }

        public void Revert(Lazurite.MainDomain.TriggerBase trigger)
        {

        }

        public event Action Modified;
        public event Action Failed;
        public event Action Succeed;
    }
}
