using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.IOC;
using Lazurite.Windows.Modules;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ComplexActionView.xaml
    /// </summary>
    public partial class ComplexActionView : VirtualizingStackPanel, IConstructorElement
    {
        private PluginsManager _manager = Singleton.Resolve<PluginsManager>();

        private ComplexAction _action;
        
        public ComplexActionView()
        {
            InitializeComponent();
        }

        public void AddFirst()
        {
            SelectCoreActionView.Show((type) => {
                var newActionHolder = new ActionHolder()
                {
                    Action = _manager.CreateInstance(type, AlgorithmContext)
                };
                _action.ActionHolders.Insert(0, newActionHolder);
                Insert(newActionHolder, 0);
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
            AlgorithmContext = algoContext;
            ActionHolder = actionHolder;
            _action = (ComplexAction)actionHolder.Action;
            Children.Clear();
            foreach (var holder in _action.ActionHolders)
                Insert(holder);
        }

        private void Insert(ActionHolder actionHolder, int position=-1)
        {
            if (position == -1)
                position = Children.Count;
            var constructorElement = ActionControlResolver.Create(actionHolder, AlgorithmContext);
            var control = ((FrameworkElement)constructorElement);
            control.Margin = new Thickness(0, 1, 0, 0);
            constructorElement.Refresh(actionHolder, AlgorithmContext);
            constructorElement.Modified += (element) => Modified?.Invoke(element);
            constructorElement.NeedRemove += (element) => {
                _action.ActionHolders.Remove(actionHolder);
                Children.Remove(control);
                Modified?.Invoke(this);
            };
            constructorElement.NeedAddNext += (element) => {
                SelectCoreActionView.Show((type) => {
                    var index = Children.IndexOf(control)+1;
                    var newActionHolder = new ActionHolder() {
                        Action = _manager.CreateInstance(type, AlgorithmContext)
                    };
                    _action.ActionHolders.Insert(index, newActionHolder);
                    Insert(newActionHolder, index);
                    Modified?.Invoke(this);
                });
            };
            Children.Insert(position, control);
        }
    }
}