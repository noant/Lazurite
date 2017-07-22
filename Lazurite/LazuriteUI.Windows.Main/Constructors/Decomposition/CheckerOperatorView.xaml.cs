using Lazurite.CoreActions.CheckerLogicalOperators;
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
using Lazurite.MainDomain;
using Lazurite.ActionsDomain;

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
            this.btSelectOperator1.Visibility =
                this.tbOperator1.Visibility = Visibility.Collapsed;
        }

        public void MakeOperatorVisible()
        {
            this.btSelectOperator1.Visibility =
                this.tbOperator1.Visibility = Visibility.Visible;
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
            Refresh(this.ActionHolder, this.AlgorithmContext);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _operatorPair = (CheckerOperatorPair)actionHolder.Action;
            this.tbOperator1.Text
                = _operatorPair.Operator == LogicalOperator.And || _operatorPair.Operator == LogicalOperator.AndNot ? "И" : "ИЛИ";
            this.tbOperator2.Text
                = _operatorPair.Operator == LogicalOperator.AndNot || _operatorPair.Operator == LogicalOperator.OrNot ? "НЕ" : string.Empty;
        }
    }
}
