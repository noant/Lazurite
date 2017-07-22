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
    public partial class ComplexCheckerOperatorPairView : Grid, IConstructorElement
    {
        private CheckerOperatorPair _pair;
        public ComplexCheckerOperatorPairView()
        {
            InitializeComponent();

            operatorView.Modified += (e) => this.Modified?.Invoke(this);
            complexCheckerView.Modified += (e) => this.Modified?.Invoke(this);

            buttonsEnd.AddNewClick += () => this.NeedAddNext?.Invoke(this);
            buttonsGroup.RemoveClick += () => this.NeedRemove?.Invoke(this);
            buttonsGroup.AddNewClick += () => complexCheckerView.AddFirst();

        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _pair = (CheckerOperatorPair)actionHolder.Action;
            operatorView.Refresh(ActionHolder, AlgorithmContext);
            complexCheckerView.Refresh(new ActionHolder((ComplexCheckerAction)_pair.Checker), algoContext);
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
            get
            {
                return complexCheckerView.AlgorithmContext;
            }
            set
            {
                complexCheckerView.AlgorithmContext = 
                    operatorView.AlgorithmContext = value;
            }
        }

        public void MakeOperatorInvisible()
        {
            this.operatorView.MakeOperatorInvisible();
        }

        public void MakeOperatorVisible()
        {
            this.operatorView.MakeOperatorVisible();
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
    }
}
