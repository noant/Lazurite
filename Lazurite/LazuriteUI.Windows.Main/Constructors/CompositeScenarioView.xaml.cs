using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Main.Constructors.Decomposition;
using System;
using System.Windows;
using System.Windows.Controls;

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

            buttons.AddNewClick += () => complexActionView.AddFirst();

            btScaleMinus.Click += (o, e) => {
                if (scale.ScaleY > 0.2)
                {
                    scale.ScaleX -= 0.1;
                    scale.ScaleY -= 0.1;
                }
            };

            btScalePlus.Click += (o, e) => {
                scale.ScaleX += 0.1;
                scale.ScaleY += 0.1;
            };

            btSettings.Click += (o, e) => {
                ActionControlResolver.BeginCompositeScenarioSettings(
                    _scenario, 
                    (result) => {
                        if (result)
                            Modified?.Invoke();
                    });
            };
            
            _scenario = scenario;
            Refresh();
            complexActionView.BeginInit();
            complexActionView.Refresh(new ActionHolder(_scenario.TargetAction), _scenario);
            complexActionView.EndInit();
            complexActionView.Modified += (element) => Modified?.Invoke();
        }
        
#pragma warning disable 67
        public event Action Failed;
        public event Action Modified;
        public event Action Succeed;
#pragma warning restore 67

        public void Revert(ScenarioBase scenario)
        {
            _scenario = (CompositeScenario)scenario;
            complexActionView.BeginInit();
            complexActionView.Refresh(new ActionHolder(_scenario.TargetAction), _scenario);
            complexActionView.EndInit();
            Refresh();
        }

        private void Refresh()
        {
            tbValueType.Text = Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(_scenario.ValueType.GetType()).ToUpper();
            btSettings.Visibility = _scenario.ValueType is ButtonValueType ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
