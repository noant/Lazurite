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
    public partial class CancelExecutionActionView : StackPanel, IConstructorElement
    {
        private CancelExecutionAction _action;

        public CancelExecutionActionView()
        {
            InitializeComponent();
            buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
            action1View.MakeButtonsInvisible();
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
            _action = (CancelExecutionAction)actionHolder.Action;
            action1View.Refresh(actionHolder, algoContext);
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
    }
}
