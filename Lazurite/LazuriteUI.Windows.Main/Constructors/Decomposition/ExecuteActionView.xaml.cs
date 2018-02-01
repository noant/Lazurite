using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ExecuteActionView.xaml
    /// </summary>
    public partial class ExecuteActionView : StackPanel, IConstructorElement
    {
        private ExecuteAction _action;

        public ExecuteActionView()
        {
            InitializeComponent();
            action1View.Modified += (element) =>
            {
                Modified?.Invoke(this);
                Action2EqualizeToAction1();
                if (!action2View.ActionHolder.Action.ValueType
                    .IsCompatibleWith(action1View.ActionHolder.Action.ValueType))
                {
                    _action.InputValue.Action = Lazurite.CoreActions.Utils.Default(action1View.ActionHolder.Action.ValueType);
                    Refresh();
                }
            };
            action2View.Modified += (element) => Modified?.Invoke(this);
            buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _action = (ExecuteAction)actionHolder.Action;
            action1View.Refresh(_action.MasterActionHolder, algoContext);
            action2View.Refresh(_action.InputValue, algoContext);
            Action2EqualizeToAction1();
        }

        public void Refresh()
        {
            Refresh(ActionHolder, AlgorithmContext);
        }

        private void Action2EqualizeToAction1()
        {
            action2View.MasterAction = action1View.ActionHolder.Action;
            if (action1View.ActionHolder.Action.ValueType is ButtonValueType)
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