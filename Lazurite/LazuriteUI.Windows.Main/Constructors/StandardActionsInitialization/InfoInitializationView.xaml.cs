using Lazurite.ActionsDomain;
using System;
using System.Windows.Controls;

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
