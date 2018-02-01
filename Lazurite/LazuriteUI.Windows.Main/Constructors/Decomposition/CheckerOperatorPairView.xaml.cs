using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.CoreActions.CheckerLogicalOperators;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для CheckerOperatorPairView.xaml
    /// </summary>
    public partial class CheckerOperatorPairView : StackPanel, IConstructorElement
    {
        private CheckerOperatorPair _pair;
        public CheckerOperatorPairView()
        {
            InitializeComponent();
            
            operatorView.Modified += (e) => Modified?.Invoke(this);
            actionView.Modified += (e) => Modified?.Invoke(this);

            actionView.NeedAddNext += (e) => NeedAddNext?.Invoke(this);
            actionView.NeedRemove += (e) => NeedRemove?.Invoke(this);

        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _pair = (CheckerOperatorPair)ActionHolder.Action;
            operatorView.Refresh(actionHolder, algoContext);
            actionView.Refresh(new ActionHolder((IAction)_pair.Checker), algoContext);
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

        public void MakeOperatorInvisible()
        {
            operatorView.MakeOperatorInvisible();
        }
        
        public void MakeOperatorVisible()
        {
            operatorView.MakeOperatorVisible();
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
    }
}
