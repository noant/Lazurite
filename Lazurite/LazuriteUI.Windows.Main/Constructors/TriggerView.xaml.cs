using Lazurite.CoreActions;
using Lazurite.Windows.Modules;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для TriggerView.xaml
    /// </summary>
    public partial class TriggerView : UserControl, ITriggerConstructorView
    {
        private Lazurite.MainDomain.TriggerBase _trigger;

        public TriggerView(Lazurite.MainDomain.TriggerBase trigger)
        {
            InitializeComponent();

            Refresh(trigger);

            buttons.AddNewClick += () => complexActionView.AddFirst();

            btScaleMinus.Click += (o, e) => {
                scale.ScaleX -= 0.1;
                scale.ScaleY -= 0.1;
            };

            btScalePlus.Click += (o, e) => {
                scale.ScaleX += 0.1;
                scale.ScaleY += 0.1;
            };

            complexActionView.Modified += (element) => Modified?.Invoke();

            btSelectScenario.Click += (o, e) => {
                SelectScenarioView.Show(
                    (selectedScenario) => {
                        if (_trigger.GetScenario() != null && !_trigger.GetScenario().ValueType.IsCompatibleWith(selectedScenario.ValueType))
                            _trigger.TargetAction = new ComplexAction();
                        _trigger.TargetScenarioId = selectedScenario.Id;
                        _trigger.SetScenario(selectedScenario);
                        Refresh(trigger);
                        Modified?.Invoke();
                    },
                    null,
                    ActionInstanceSide.OnlyRight,
                    _trigger.TargetScenarioId
                );
            };
        }

        public void Revert(Lazurite.MainDomain.TriggerBase trigger)
        {
            _trigger = trigger;
            tbSelectedScenario.Text = trigger.GetScenario()?.Name ?? "[сценарий не выбран]";
            complexActionView.Refresh(new ActionHolder(trigger.TargetAction), trigger);
            buttons.IsEnabled = trigger.GetScenario() != null;
        }

        public void Refresh(Lazurite.MainDomain.TriggerBase trigger)
        {
            Revert(trigger);
        }

#pragma warning disable 67
        public event Action Modified;
        public event Action Failed;
        public event Action Succeed;
#pragma warning restore 67
    }
}
