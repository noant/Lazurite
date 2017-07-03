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
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для CompositeScenarioView.xaml
    /// </summary>
    public partial class CompositeScenarioView : Grid, IScenarioConstructorView
    {
        private CompositeScenario _scenario;

        public CompositeScenarioView(CompositeScenario scenario)
        {
            InitializeComponent();

            btScaleMinus.Click += (o, e) => {
                scale.ScaleX -= 0.1;
                scale.ScaleY -= 0.1;
            };

            btScalePlus.Click += (o, e) => {
                scale.ScaleX += 0.1;
                scale.ScaleY += 0.1;
            };

            complexActionView.MakeRemoveButtonInvisible();
            _scenario = scenario;
            complexActionView.Refresh(_scenario.TargetAction);
            complexActionView.Modified += (element) => Modified?.Invoke();
        }

        public event Action Failed;
        public event Action Modified;
        public event Action Succeed;

        public void Revert(ScenarioBase scenario)
        {
            _scenario = (CompositeScenario)scenario;
            complexActionView.Refresh(_scenario.TargetAction);
        }
    }
}
