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
using Lazurite.ActionsDomain;
using Lazurite.Windows.Modules;
using Lazurite.IOC;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ActionView.xaml
    /// </summary>
    public partial class ActionView : UserControl, IConstructorElement
    {
        public ActionView(IAction action)
        {
            InitializeComponent();
            Action = action;
            Model = new ActionModel();
            Model.Refresh(Action);
            DataContext = Model;

            buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
            buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            buttons.EditClick += () =>
            {
                if (Action.UserInitializeWith(MasterAction?.ValueType, true))
                    Modified?.Invoke(this);
            };
            buttons.ChangeClick += () => {
                SelectActionView.Show(
                    (type) => {
                        var newAction = Singleton.Resolve<PluginsManager>().CreateInstanceOf(type);
                        if (newAction.UserInitializeWith(MasterAction?.ValueType, MasterAction != null))
                        {
                            Action = newAction;
                            Model.Refresh(Action);
                            Modified?.Invoke(this);
                        }
                    },
                    Window.GetWindow(this).Content as Panel,
                    MasterAction?.ValueType.GetType(),
                    MasterAction == null ? Lazurite.Windows.Modules.ActionInstanceSide.OnlyLeft
                    : Lazurite.Windows.Modules.ActionInstanceSide.OnlyRight, 
                    Action?.GetType());
            };
        }

        public IAction MasterAction { get; private set; }

        public ActionModel Model
        {
            get; private set;
        }

        public IAction Action
        {
            get; private set;
        }

        public bool EditMode
        {
            get
            {
                return Model.EditMode;
            }
            set
            {
                Model.EditMode = value;
            }
        }

        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
        public event Action<IConstructorElement> Modified;
    }
}
