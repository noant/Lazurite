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
using Lazurite.CoreActions;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ActionView.xaml
    /// </summary>
    public partial class ActionView : UserControl, IConstructorElement
    {
        public ActionView() : this(new ActionHolder())
        {
            // do nothing
        }

        public ActionView(ActionHolder actionHolder)
        {
            InitializeComponent();

            Refresh(actionHolder);

            buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
            buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            buttons.EditClick += () =>
            {
                ActionControlResolver.UserInitialize(
                    (result) => {
                        if (result)
                        {
                            Model.Refresh(ActionHolder);
                            Modified?.Invoke(this);
                        }
                    },
                    ActionHolder.Action,
                    MasterAction?.ValueType,
                    MasterAction != null,
                    MasterAction);
            };
            buttons.ChangeClick += () => {
                BeginSelectAction();
            };
        }

        public void BeginSelectAction()
        {
            SelectActionView.Show(
                    (type) => {
                        var newAction = Singleton.Resolve<PluginsManager>().CreateInstanceOf(type);
                        if (newAction != null)
                        {
                            ActionControlResolver.UserInitialize(
                                (result) => {
                                    if (result)
                                    {
                                        Model.Refresh(ActionHolder);
                                        Modified?.Invoke(this);
                                        ActionHolder.Action = newAction;
                                        Model.Refresh();
                                        Modified?.Invoke(this);
                                    }
                                },
                                newAction,
                                MasterAction?.ValueType,
                                true,
                                MasterAction);
                        }
                    },
                    MasterAction?.ValueType.GetType(),
                    MasterAction == null ? Lazurite.Windows.Modules.ActionInstanceSide.OnlyLeft
                    : Lazurite.Windows.Modules.ActionInstanceSide.OnlyRight,
                    ActionHolder?.Action.GetType());
        }

        public void Refresh(ActionHolder actionHolder)
        {
            ActionHolder = actionHolder;
            Model = new ActionModel();
            Model.Refresh(ActionHolder);
            DataContext = Model;
        }

        public void Refresh()
        {
            this.Refresh(this.ActionHolder);
        }

        public IAction MasterAction { get; set; }

        public ActionModel Model
        {
            get; private set;
        }

        public ActionHolder ActionHolder
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
