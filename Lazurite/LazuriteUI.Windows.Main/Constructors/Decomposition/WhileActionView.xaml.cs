using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для IfActionView.xaml
    /// </summary>
    public partial class WhileActionView : Grid, IConstructorElement
    {
        private WhileAction _action;

        public WhileActionView()
        {
            InitializeComponent();
            buttonsEnd.AddNewClick += () => NeedAddNext?.Invoke(this);
            buttonsWhile.RemoveClick += () => NeedRemove?.Invoke(this);
            buttonsDo.AddNewClick += () => actionView.AddFirst();
            buttonsWhile.AddNewClick += () => checkerView.AddFirst();
            
            actionView.Modified += (e) => Modified?.Invoke(this);
            checkerView.Modified += (e) => Modified?.Invoke(this);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _action = (WhileAction)actionHolder.Action;
            actionView.Refresh(new ActionHolder(_action.Action), algoContext);
            checkerView.Refresh(new ActionHolder(_action.Checker), algoContext);
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
