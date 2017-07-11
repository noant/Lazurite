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
using Lazurite.MainDomain;
using Lazurite.ActionsDomain;
using Lazurite.CoreActions.ComparisonTypes;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для CheckerActionView.xaml
    /// </summary>
    public partial class CheckerActionView : UserControl, IConstructorElement
    {
        private CheckerAction _action;

        public CheckerActionView()
        {
            InitializeComponent();
            
            this.action1View.Modified += (element) =>
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
            this.action2View.Modified += (element) => Modified?.Invoke(this);
            this.comparisonView.Modified += (element) => Modified?.Invoke(this);
            this.buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            this.buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            this.AlgorithmContext = algoContext;
            this.ActionHolder = actionHolder;
            _action = (CheckerAction)actionHolder.Action;
            this.action1View.Refresh(_action.TargetAction1Holder, algoContext);
            this.action2View.Refresh(_action.TargetAction2Holder, algoContext);
            this.comparisonView.Refresh(actionHolder, algoContext);
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