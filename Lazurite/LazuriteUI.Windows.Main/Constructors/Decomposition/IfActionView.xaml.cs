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
using Lazurite.ActionsDomain;
using Lazurite.CoreActions;

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
