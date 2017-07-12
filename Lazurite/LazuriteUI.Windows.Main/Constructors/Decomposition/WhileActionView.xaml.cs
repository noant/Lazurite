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
    public partial class WhileActionView : UserControl, IConstructorElement
    {
        private WhileAction _action;

        public WhileActionView()
        {
            InitializeComponent();
            buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
            buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            
            this.actionView.Modified += (e) => Modified?.Invoke(this);
            this.checkerView.Modified += (e) => Modified?.Invoke(this);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _action = (WhileAction)actionHolder.Action;
            this.actionView.Refresh(new ActionHolder(_action.Action), algoContext);
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
