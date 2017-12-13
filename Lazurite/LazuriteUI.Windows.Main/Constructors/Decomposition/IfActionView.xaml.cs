using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для IfActionView.xaml
    /// </summary>
    public partial class IfActionView : Grid, IConstructorElement
    {
        private IfAction _action;

        public IfActionView()
        {
            InitializeComponent();
            buttonsEnd.AddNewClick += () => NeedAddNext?.Invoke(this);
            buttonsChecker.RemoveClick += () => NeedRemove?.Invoke(this);
            buttonsChecker.AddNewClick += () => checkerView.AddFirst();
            buttonsIf.AddNewClick += () => actionIfView.AddFirst();
            buttonsElse.AddNewClick += () => actionElseView.AddFirst();

            this.actionElseView.Modified += (e) => Modified?.Invoke(this);
            this.actionIfView.Modified += (e) => Modified?.Invoke(this);
            this.checkerView.Modified += (e) => Modified?.Invoke(this);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _action = (IfAction)actionHolder.Action;
            this.actionIfView.Refresh(new ActionHolder(_action.ActionIf), algoContext);
            this.actionElseView.Refresh(new ActionHolder(_action.ActionElse), algoContext);
            this.checkerView.Refresh(new ActionHolder(_action.Checker), algoContext);
        }

        public ActionHolder ActionHolder
        {
            get;
            private set;
        }

        public IAlgorithmContext AlgorithmContext
        {
            get;
            private set;
        }

        public bool EditMode
        {
            get;
            set;
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
    }
}
