using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.CoreActions.ComparisonTypes;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для CheckerActionView.xaml
    /// </summary>
    public partial class CheckerActionView : StackPanel, IConstructorElement
    {
        private CheckerAction _action;

        public CheckerActionView()
        {
            InitializeComponent();
            
            action1View.Modified += (element) =>
            {
                Modified?.Invoke(this);
                Action2EqualizeToAction1();
                if (action2View.ActionHolder.Action.ValueType.GetType() !=
                    action1View.ActionHolder.Action.ValueType.GetType())
                {
                    _action.ComparisonType = new EqualityComparisonType();
                    _action.TargetAction2Holder.Action = Lazurite.CoreActions.Utils.Default(action1View.ActionHolder.Action.ValueType);
                    action2View.Refresh();
                    comparisonView.Refresh();
                }
            };
            action2View.Modified += (element) => Modified?.Invoke(this);
            comparisonView.Modified += (element) => Modified?.Invoke(this);
            buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            AlgorithmContext = algoContext;
            ActionHolder = actionHolder;
            _action = (CheckerAction)actionHolder.Action;
            action1View.Refresh(_action.TargetAction1Holder, algoContext);
            action2View.Refresh(_action.TargetAction2Holder, algoContext);
            comparisonView.Refresh(actionHolder, algoContext);
            Action2EqualizeToAction1();
        }

        private void Action2EqualizeToAction1()
        {
            action2View.MasterAction = action1View.ActionHolder.Action;
        }

        public ActionHolder ActionHolder
        {
            get;
            private set;
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