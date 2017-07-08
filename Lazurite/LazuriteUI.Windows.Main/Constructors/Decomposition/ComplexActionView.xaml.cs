using Lazurite.CoreActions;
using Lazurite.CoreActions.ContextInitialization;
using Lazurite.CoreActions.CoreActions;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Windows.Modules;
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

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ComplexActionView.xaml
    /// </summary>
    public partial class ComplexActionView : UserControl, IConstructorElement
    {
        private PluginsManager _manager = Singleton.Resolve<PluginsManager>();

        private ComplexAction _action;
        
        public ComplexActionView(ComplexAction action, ScenarioBase scenario)
        {
            InitializeComponent();
            buttons.AddNewClick += () =>
            {
                SelectCoreActionView.Show((type) => {
                    var newActionHolder = new ActionHolder()
                    {
                        Action = _manager.CreateInstanceOf(type, ParentScenario)
                    };
                    _action.ActionHolders.Insert(0, newActionHolder);
                    Insert(newActionHolder, 0);
                    Modified?.Invoke(this);
                });
            };
            buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            Refresh(action);
        }

        public ComplexActionView() : this(new ComplexAction(), null)
        {
            //do nothing
        }

        public ActionHolder ActionHolder
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool EditMode
        {
            get;
            set;
        }

        public ScenarioBase ParentScenario
        {
            get;
            set;
        }

        public event Action<IConstructorElement> Modified;
        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;

        public void Refresh(ComplexAction action)
        {
            _action = action;
            stackPanel.Children.Clear();
            foreach (var actionHolder in _action.ActionHolders)
                Insert(actionHolder);
        }

        private void Insert(ActionHolder actionHolder, int position=-1)
        {
            if (position == -1)
                position = stackPanel.Children.Count;
            var control = ActionControlResolver.Create(actionHolder.Action);
            ((FrameworkElement)control).Margin = new Thickness(0, 1, 0, 0);
            var constructorElement = control as IConstructorElement;
            constructorElement.ParentScenario = this.ParentScenario;
            constructorElement.Modified += (element) => Modified?.Invoke(element);
            constructorElement.NeedRemove += (element) => {
                _action.ActionHolders.Remove(actionHolder);
                stackPanel.Children.Remove(control);
                Modified?.Invoke(this);
            };
            constructorElement.NeedAddNext += (element) => {
                SelectCoreActionView.Show((type) => {
                    var index = stackPanel.Children.IndexOf(control)+1;
                    var newActionHolder = new ActionHolder() {
                        Action = _manager.CreateInstanceOf(type, ParentScenario)
                    };
                    _action.ActionHolders.Insert(index, newActionHolder);
                    Insert(newActionHolder, index);
                    Modified?.Invoke(this);
                });
            };
            stackPanel.Children.Insert(position, control);
        }

        public void MakeRemoveButtonInvisible()
        {
            this.buttons.RemoveVisible = false;
        }
    }
}