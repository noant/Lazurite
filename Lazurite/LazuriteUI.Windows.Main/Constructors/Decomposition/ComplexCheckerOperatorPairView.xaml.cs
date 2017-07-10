using Lazurite.CoreActions;
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
using Lazurite.MainDomain;
using Lazurite.ActionsDomain;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для CheckerOperatorPairView.xaml
    /// </summary>
    public partial class ComplexCheckerOperatorPairView : StackPanel, IConstructorElement
    {
        private CheckerOperatorPair _pair;
        public ComplexCheckerOperatorPairView(CheckerOperatorPair pair)
        {
            InitializeComponent();

            _pair = pair;
            
            operatorView.Modified += (e) => this.Modified?.Invoke(this);
            complexCheckerView.Modified += (e) => this.Modified?.Invoke(this);

            buttons.AddNewClick += () => this.NeedAddNext?.Invoke(this);
            buttons.RemoveClick += () => this.NeedRemove?.Invoke(this);

            operatorView.Refresh(pair);
            complexCheckerView.Refresh((ComplexCheckerAction)pair.Checker);
        }

        public ActionHolder ActionHolder
        {
            get
            {
                return new ActionHolder() { Action = (IAction)_pair?.Checker };
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
                return complexCheckerView.AlgorithmContext;
            }
            set
            {
                complexCheckerView.AlgorithmContext = operatorView.AlgorithmContext = value;
            }
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
    }
}
