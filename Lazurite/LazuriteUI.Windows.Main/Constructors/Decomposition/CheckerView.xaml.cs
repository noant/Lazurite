using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.IOC;
using Lazurite.Windows.Modules;
using LazuriteUI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для ActionView.xaml
    /// </summary>
    public partial class CheckerView : Grid, IConstructorElement
    {
        public CheckerView()
        {
            InitializeComponent();
            
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
                        var newAction = Singleton.Resolve<PluginsManager>().CreateInstance(type, AlgorithmContext);
                        if (newAction != null)
                        {
                            ActionControlResolver.UserInitialize(
                                (result) => {
                                    if (result)
                                    {
                                        ActionHolder.Action = newAction;
                                        Model.Refresh(ActionHolder);
                                        Modified?.Invoke(this);
                                    }
                                },
                                newAction,
                                MasterAction?.ValueType,
                                true,
                                MasterAction);
                            if (MasterAction != null && MasterAction.ValueType.GetType() != newAction.ValueType.GetType())
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
                    ActionInstanceSide.OnlyRight,
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
                        if (MasterAction != null && MasterAction.ValueType.GetType() != ActionHolder.Action.ValueType.GetType())
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
            AlgorithmContext = algoContext;
            ActionHolder = actionHolder;
            Model = new ActionModel();
            Model.Refresh(ActionHolder);
            DataContext = Model;
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
            set;
        }

        public event Action<IConstructorElement> NeedAddNext;
        public event Action<IConstructorElement> NeedRemove;
        public event Action<IConstructorElement> Modified;
    }
}
