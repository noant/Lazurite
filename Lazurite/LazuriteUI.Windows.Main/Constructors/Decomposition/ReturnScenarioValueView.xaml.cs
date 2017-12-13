using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.CoreActions.ContextInitialization;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ExecuteActionView.xaml
    /// </summary>
    public partial class ReturnScenarioValueView : StackPanel, IConstructorElement
    {
        private SetReturnValueAction _action;

        public ReturnScenarioValueView()
        {
            InitializeComponent();
            this.action2View.Modified += (element) => Modified?.Invoke(this);
            this.buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            this.buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
            this.action1View.MakeButtonsInvisible();
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

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _action = (SetReturnValueAction)actionHolder.Action;
            this.action1View.Refresh(actionHolder, algoContext);
            this.action2View.Refresh(_action.InputValue, algoContext);
            this.action2View.MasterAction = _action;
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
    }
}
