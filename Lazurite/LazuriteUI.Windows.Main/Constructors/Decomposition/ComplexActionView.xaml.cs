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
                    Action = _manager.CreateInstanceOf(type, AlgorithmContext)
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

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;

        public void Refresh()
        {
            Refresh(this.ActionHolder, this.AlgorithmContext);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            AlgorithmContext = algoContext;
            ActionHolder = actionHolder;
            _action = (ComplexAction)actionHolder.Action;
            this.Children.Clear();
            foreach (var holder in _action.ActionHolders)
                Insert(holder);
        }

        private void Insert(ActionHolder actionHolder, int position=-1)
        {
            if (position == -1)
                position = this.Children.Count;
            var constructorElement = ActionControlResolver.Create(actionHolder, this.AlgorithmContext);
            var control = ((FrameworkElement)constructorElement);
            control.Margin = new Thickness(0, 1, 0, 0);
            constructorElement.Refresh(actionHolder, this.AlgorithmContext);
            constructorElement.Modified += (element) => Modified?.Invoke(element);
            constructorElement.NeedRemove += (element) => {
                _action.ActionHolders.Remove(actionHolder);
                this.Children.Remove(control);
                Modified?.Invoke(this);
            };
            constructorElement.NeedAddNext += (element) => {
                SelectCoreActionView.Show((type) => {
                    var index = this.Children.IndexOf(control)+1;
                    var newActionHolder = new ActionHolder() {
                        Action = _manager.CreateInstanceOf(type, this.AlgorithmContext)
                    };
                    _action.ActionHolders.Insert(index, newActionHolder);
                    Insert(newActionHolder, index);
                    Modified?.Invoke(this);
                });
            };
            this.Children.Insert(position, control);
        }
    }
}