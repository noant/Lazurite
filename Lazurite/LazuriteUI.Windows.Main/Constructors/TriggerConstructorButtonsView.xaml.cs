using LazuriteUI.Windows.Controls;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для ScenarioConstructorButtonsView.xaml
    /// </summary>
    public partial class TriggerConstructorButtonsView : UserControl, ITriggerConstructorView
    {
        private Lazurite.MainDomain.TriggerBase _trigger;

        public TriggerConstructorButtonsView()
        {
            InitializeComponent();
            btApply.Click += (o, e) =>
            {
                MessageView.ShowYesNo("Вы уверены что хотите применить изменения триггера?", "Изменения триггера", Icons.Icon.Question,
                    (result) => {
                        if (result)
                        {
                            ApplyClicked?.Invoke();
                            btCancel.IsEnabled = btApply.IsEnabled = false;
                        }
                    });
            };

            btCancel.Click += (o, e) =>
            {
                MessageView.ShowYesNo("Вы уверены что хотите отменить все изменения триггера?", "Изменения триггера", Icons.Icon.Question,
                    (result) => {
                        if (result)
                            ResetClicked?.Invoke();
                    });
            };

            tbName.TextChanged += (o, e) =>
            {
                _trigger.Name = tbName.Text;
                TriggerModified();
            };

            btEnabled.SelectionChanged += (o, e) => {
                _trigger.Enabled = btEnabled.Selected;
                TriggerModified();
            };
        }

        event Action ITriggerConstructorView.Failed
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

        public void TriggerModified()
        {
            btCancel.IsEnabled = btApply.IsEnabled = true;
            Modified?.Invoke();
        }

        public void SetTrigger(Lazurite.MainDomain.TriggerBase trigger)
        {
            _trigger = trigger;
            Refresh();
        }

        private void Refresh()
        {
            tbName.Text = _trigger.Name;
            btEnabled.Selected = _trigger.Enabled;
            btCancel.IsEnabled = btApply.IsEnabled = false;
        }

        public void Revert(Lazurite.MainDomain.TriggerBase trigger)
        {
            _trigger = trigger;
            Refresh();
        }

        public void Failed()
        {
            btApply.IsEnabled = false;
        }

        public void Success()
        {
            btApply.IsEnabled = true;
        }

        public event Action ApplyClicked;
        public event Action ResetClicked;
        public event Action Modified;
        public event Action Succeed;
    }
}
