using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
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
    /// Логика взаимодействия для ScenarioConstructorButtonsView.xaml
    /// </summary>
    public partial class ScenarioConstructorButtonsView : UserControl, IScenarioConstructorView
    {
        private ScenarioBase _scenario;

        public ScenarioConstructorButtonsView()
        {
            InitializeComponent();
            btApply.Click += (o, e) =>
            {
                MessageView.ShowYesNo("Вы уверены, что хотите применить изменения сценария?", "Изменения сценария", Icons.Icon.Question,
                    (result) => {
                        ApplyClicked?.Invoke();
                        btCancel.IsEnabled = btApply.IsEnabled = false;
                    });
            };

            btCancel.Click += (o, e) =>
            {
                MessageView.ShowYesNo("Вы уверены, что хотите отменить все изменения сценария?", "Изменения сценария", Icons.Icon.Question,
                    (result) => {
                        ResetClicked?.Invoke();
                    });
            };

            tbName.TextChanged += (o, e) =>
            {
                _scenario.Name = tbName.Text;
                ScenarioModified();
            };
        }

        event Action IScenarioConstructorView.Failed
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public void ScenarioModified()
        {
            btCancel.IsEnabled = btApply.IsEnabled = true;
        }

        public void SetScenario(ScenarioBase scenario)
        {
            _scenario = scenario;
            Refresh();
        }

        private void Refresh()
        {
            tbName.Text = _scenario.Name;
            btCancel.IsEnabled = btApply.IsEnabled = false;
        }

        public void Revert(ScenarioBase scenario)
        {
            _scenario = scenario;
            Refresh();
        }

        public void Failed()
        {
            this.btApply.IsEnabled = false;
        }

        public void Success()
        {
            this.btApply.IsEnabled = true;
        }

        public event Action ApplyClicked;
        public event Action ResetClicked;
        public event Action Modified;
        public event Action Succeed;
    }
}
