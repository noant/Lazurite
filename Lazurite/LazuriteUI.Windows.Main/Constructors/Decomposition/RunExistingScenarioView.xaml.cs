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
using Lazurite.CoreActions;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.MainDomain;
using Lazurite.ActionsDomain;
using Lazurite.CoreActions.CoreActions;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ExecuteActionView.xaml
    /// </summary>
    public partial class RunExistingScenarioView : StackPanel, IConstructorElement
    {
        private RunExistingScenarioAction _action;

        public RunExistingScenarioView()
        {
            InitializeComponent();
            this.action1View.Modified += (element) =>
            {
                Modified?.Invoke(this);
                Action2EqualizeToAction1();
                if (_action.GetTargetScenario() != null && 
                    !_action.GetTargetScenario().ValueType
                    .IsCompatibleWith(_action.InputValue.Action.ValueType))
                {
                    _action.InputValue.Action = Lazurite.CoreActions.Utils.Default(_action.GetTargetScenario().ValueType);
                    action2View.Refresh();
                }
            };
            this.action2View.Modified += (element) => Modified?.Invoke(this);
            this.buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            this.buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
            this.action1View.MakeChangeButtonInvisible();
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _action = (RunExistingScenarioAction)actionHolder.Action;
            this.action1View.Refresh(new ActionHolder(_action), algoContext);
            this.action2View.Refresh(_action.InputValue, algoContext);
            Action2EqualizeToAction1();
        }

        private void Action2EqualizeToAction1()
        {
            //crutch begin
            var masterAction = new GetExistingScenarioValueAction();
            masterAction.SetTargetScenario(_action.GetTargetScenario());
            masterAction.TargetScenarioId = _action.TargetScenarioId;
            //crutch end

            action2View.MasterAction = masterAction;
            if (masterAction.ValueType is ButtonValueType)
                action2View.Visibility = tbEquals.Visibility = Visibility.Collapsed;
            else action2View.Visibility = tbEquals.Visibility = Visibility.Visible;
        }

        public ActionHolder ActionHolder
        {
            get; private set;
        }

        public bool EditMode
        {
            get;
            set;
        }

        public IAlgorithmContext AlgorithmContext
        {
            get;
            private set;
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
    }
}