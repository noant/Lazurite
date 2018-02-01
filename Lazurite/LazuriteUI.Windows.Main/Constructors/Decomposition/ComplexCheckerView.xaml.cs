using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.CoreActions.CheckerLogicalOperators;
using Lazurite.IOC;
using Lazurite.Windows.Modules;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ComplexCheckerView.xaml
    /// </summary>
    public partial class ComplexCheckerView : VirtualizingStackPanel, IConstructorElement
    {
        private PluginsManager _manager = Singleton.Resolve<PluginsManager>();

        private ComplexCheckerAction _action;

        public ComplexCheckerView()
        {
            InitializeComponent();
        }

        public void AddFirst()
        {
            SelectCheckerTypeView.Show((isGroup) => {
                CheckerOperatorPair operatorPair = new CheckerOperatorPair();
                if (isGroup)
                    operatorPair.Checker = new ComplexCheckerAction();
                _action.CheckerOperations.Insert(0, operatorPair);
                Insert(operatorPair, 0);
                Modified?.Invoke(this);
            });
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
            set;
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            AlgorithmContext = algoContext;
            ActionHolder = actionHolder;
            _action = (ComplexCheckerAction)actionHolder.Action;
            Children.Clear();
            foreach (var pair in _action.CheckerOperations)
                Insert(pair);
        }

        private void Insert(CheckerOperatorPair operatorPair, int position = -1)
        {
            if (position == -1)
                position = Children.Count;
            FrameworkElement control = null;
            if (operatorPair.Checker is CheckerAction)
                control = new CheckerOperatorPairView();
            else if (operatorPair.Checker is ComplexCheckerAction)
                control = new ComplexCheckerOperatorPairView();
            var constructorElement = ((IConstructorElement)control);
            control.Margin = new Thickness(0, 1, 0, 0);
            constructorElement.Refresh(new ActionHolder(operatorPair), AlgorithmContext);
            constructorElement.Modified += (element) => Modified?.Invoke(element);
            constructorElement.NeedRemove += (element) => {
                _action.CheckerOperations.Remove(operatorPair);
                Children.Remove(control);
                Modified?.Invoke(this);
                MakeFirstRowOperatorInvisible();
            };
            constructorElement.NeedAddNext += (element) => {
                SelectCheckerTypeView.Show((isGroup) => {
                    var index = Children.IndexOf(control) + 1;
                    CheckerOperatorPair newOperatorPair = new CheckerOperatorPair();
                    if (isGroup)
                        newOperatorPair.Checker = new ComplexCheckerAction();
                    _action.CheckerOperations.Insert(index, newOperatorPair);
                    Insert(newOperatorPair, index);
                    Modified?.Invoke(this);
                });
            };
            Children.Insert(position, control);
            MakeFirstRowOperatorInvisible();
        }

        private void MakeFirstRowOperatorInvisible()
        {
            if (Children.Count > 0)
            {
                foreach (var control in Children)
                {
                    (control as CheckerOperatorPairView)?.MakeOperatorVisible();
                    (control as ComplexCheckerOperatorPairView)?.MakeOperatorVisible();
                }
                (Children[0] as CheckerOperatorPairView)?.MakeOperatorInvisible();
                (Children[0] as ComplexCheckerOperatorPairView)?.MakeOperatorInvisible();
            }
        }
    }
}
