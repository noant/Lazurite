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
using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ActionView.xaml
    /// </summary>
    public partial class ActionView : UserControl, IConstructorElement
    {
        public ActionView()
        {
            InitializeComponent();
            
            buttons.AddNewClick += () => NeedAddNext?.Invoke(this);
            buttons.RemoveClick += () => NeedRemove?.Invoke(this);
            buttons.EditClick += () => BeginEditAction();
            buttons.ChangeClick += () => BeginSelectAction();
        }

        public void MakeButtonsInvisible()
        {
            buttons.Visibility = Visibility.Collapsed;
        }

        public void BeginSelectAction()
        {
            SelectActionView.Show(
                    (type) => {
                        var newAction = Singleton.Resolve<PluginsManager>().CreateInstanceOf(type, AlgorithmContext);
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
                            if (MasterAction != null && !MasterAction.ValueType.IsCompatibleWith(newAction.ValueType))
                            {
                                MessageView.ShowMessage(
                                    "Тип действия не совпадает с типом действия главного действия. Нужно настроить подчиненное действие еще раз.",
                                    "Внимание!",
                                    Icons.Icon.WarningCircle, null, 
                                    () => {
                                        BeginSelectAction();
                                    });
                            }
                        }
                    },
                    MasterAction?.ValueType.GetType(),
                    MasterAction == null ? Lazurite.Windows.Modules.ActionInstanceSide.OnlyLeft
                    : Lazurite.Windows.Modules.ActionInstanceSide.OnlyRight,
                    ActionHolder?.Action.GetType());
        }

        public void BeginEditAction()
        {
            ActionControlResolver.UserInitialize(
                (result) => {
                    if (result)
                    {
                        Model.Refresh(ActionHolder);
                        Modified?.Invoke(this);
                        if (MasterAction != null && !MasterAction.ValueType.IsCompatibleWith(ActionHolder.Action.ValueType))
                        {
                            MessageView.ShowMessage(
                                "Тип действия не совпадает с типом главного действия. Нужно настроить подчиненное действие еще раз.",
                                "Внимание!",
                                Icons.Icon.WarningCircle, null,
                                () => BeginEditAction()
                            );
                        }
                    }
                },
                ActionHolder.Action,
                MasterAction?.ValueType,
                MasterAction != null,
                MasterAction);
        }

        public void Refresh(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            ActionHolder = actionHolder;
            this.AlgorithmContext = algoContext;
            if (Model == null)
            {
                Model = new ActionModel();
                DataContext = Model;
            }
            Model.Refresh(ActionHolder);
        }

        public void Refresh()
        {
            this.Refresh(this.ActionHolder, this.AlgorithmContext);
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

        public IAlgorithmContext AlgorithmContext
        {
            get;
            private set;
        }

        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
        public event Action<IConstructorElement> Modified;
    }
}
