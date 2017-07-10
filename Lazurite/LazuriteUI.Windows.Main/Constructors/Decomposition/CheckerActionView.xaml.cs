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
                    _action.TargetAction2Holder.Action = Lazurite.CoreActions.Utils.Default(action1View.ActionHolder.Action.ValueType);
                    action2View.Refresh();
                }
            };
            this.action2View.Modified += (element) => Modified?.Invoke(this);
            this.buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            this.buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
        }

        public void Refresh(CheckerAction action)
        {
            _action = action;
            this.action1View.Refresh(action.TargetAction1Holder);
            this.action2View.Refresh(action.TargetAction2Holder);
            this.comparisonView.Refresh(action);
            Action2EqualizeToAction1();
        }

        private void Action2EqualizeToAction1()
        {
            action2View.MasterAction = action1View.ActionHolder.Action;
        }

        public ActionHolder ActionHolder
        {
            get {
                return new ActionHolder() { Action = _action };
            }
        }

        public bool EditMode
        {
            get;
            set;
        }

        public IAlgorithmContext AlgorithmContext
        {
            get
            {
                return action1View.AlgorithmContext;
            }
            set
            {
                action1View.AlgorithmContext
                    = action2View.AlgorithmContext
                    = comparisonView.AlgorithmContext = value;
            }
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
    }
}