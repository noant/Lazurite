using Lazurite.ActionsDomain;
using Lazurite.CoreActions.StandardValueTypeActions;
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

namespace LazuriteUI.Windows.Main.Constructors.StandardActionsInitialization
{
    /// <summary>
    /// Логика взаимодействия для InfoInitializationView.xaml
    /// </summary>
    public partial class InfoInitializationView : UserControl, IStandardVTActionEditView
    {
        private IStandardValueAction _action;

        public InfoInitializationView(IStandardValueAction action, IAction masterAction = null)
        {
            InitializeComponent();

            _action = action;

            this.tbText.Text = _action.Value;

            btApply.Click += (o, e) => {
                _action.Value = tbText.Text;
                ApplyClicked?.Invoke();
            };
        }

        public event Action ApplyClicked;
    }
}
