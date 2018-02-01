using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.CoreActions.CheckerLogicalOperators;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для CheckerOperator.xaml
    /// </summary>
    public partial class CheckerOperatorView : Grid, IConstructorElement
    {
        private CheckerOperatorPair _operatorPair;

        public CheckerOperatorView()
        {
            InitializeComponent();

            btSelectOperator1.Click += (o, e) => {
                CheckerOperatorSelectView.Show(
                    (orSelected) => {
                        if (orSelected)
                        {
                            if (_operatorPair.Operator == LogicalOperator.And)
                                _operatorPair.Operator = LogicalOperator.Or;
                            else if (_operatorPair.Operator == LogicalOperator.AndNot)
                                _operatorPair.Operator = LogicalOperator.OrNot;
                        }
                        else
                        {
                            if (_operatorPair.Operator == LogicalOperator.Or)
                                _operatorPair.Operator = LogicalOperator.And;
                            else if (_operatorPair.Operator == LogicalOperator.OrNot)
                                _operatorPair.Operator = LogicalOperator.AndNot;
                        }
                        Refresh();
                        Modified?.Invoke(this);
                    }, 
                    _operatorPair.Operator == LogicalOperator.Or || _operatorPair.Operator == LogicalOperator.OrNot);
            };

            btSelectOperator2.Click += (o, e) => {
                CheckerOperatorSelectView2.Show(
                    (notSelected) => {
                        if (notSelected)
                        {
                            if (_operatorPair.Operator == LogicalOperator.And)
                                _operatorPair.Operator = LogicalOperator.AndNot;
                            else if (_operatorPair.Operator == LogicalOperator.Or)
                                _operatorPair.Operator = LogicalOperator.OrNot;
                        }
                        else
                        {
                            if (_operatorPair.Operator == LogicalOperator.AndNot)
                                _operatorPair.Operator = LogicalOperator.And;
                            else if (_operatorPair.Operator == LogicalOperator.OrNot)
                                _operatorPair.Operator = LogicalOperator.Or;
                        }
                        Refresh();
                        Modified?.Invoke(this);
                    },
                    _operatorPair.Operator == LogicalOperator.AndNot || _operatorPair.Operator == LogicalOperator.OrNot);
            };
        }

        public void MakeOperatorInvisible()
        {
            btSelectOperator1.Visibility =
                tbOperator1.Visibility = Visibility.Collapsed;
        }

        public void MakeOperatorVisible()
        {
            btSelectOperator1.Visibility =
                tbOperator1.Visibility = Visibility.Visible;
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

        public void Refresh()
        {
            Refresh(ActionHolder, AlgorithmContext);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _operatorPair = (CheckerOperatorPair)actionHolder.Action;
            tbOperator1.Text
                = _operatorPair.Operator == LogicalOperator.And || _operatorPair.Operator == LogicalOperator.AndNot ? "И" : "ИЛИ";
            tbOperator2.Text
                = _operatorPair.Operator == LogicalOperator.AndNot || _operatorPair.Operator == LogicalOperator.OrNot ? "НЕ" : string.Empty;
        }
    }
}
