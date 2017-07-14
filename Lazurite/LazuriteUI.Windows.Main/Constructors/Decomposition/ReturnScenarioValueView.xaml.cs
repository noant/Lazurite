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
using Lazurite.CoreActions.CoreActions;
using Lazurite.MainDomain;
using Lazurite.CoreActions.ContextInitialization;
using Lazurite.ActionsDomain;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ExecuteActionView.xaml
    /// </summary>
    public partial class ReturnScenarioValueView : UserControl, IConstructorElement
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
