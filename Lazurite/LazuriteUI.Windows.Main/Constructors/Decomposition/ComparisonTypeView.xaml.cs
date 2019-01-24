using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ComparisonTypeView.xaml
    /// </summary>
    public partial class ComparisonTypeView : Grid, IConstructorElement
    {
        private CheckerAction _checkerAction;

        public ComparisonTypeView()
        {
            InitializeComponent();

            button.Click += (o, e) => {
                ComparisonTypeSelectView.Show(
                    (type) => {
                        _checkerAction.ComparisonType = type;
                        Refresh();
                        Modified?.Invoke(this);
                    },
                    _checkerAction.ComparisonType,
                    _checkerAction.TargetAction1Holder.Action.ValueType.SupportsNumericalComparisons);
            };
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

#pragma warning disable 67
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
        public event Action<IConstructorElement> Modified;
#pragma warning restore 67

        public void Refresh()
        {
            Refresh(ActionHolder, AlgorithmContext);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            AlgorithmContext = algoContext;
            _checkerAction = (CheckerAction)actionHolder.Action;
            textBlock.Text = _checkerAction.ComparisonType.Caption;
        }
    }
}
