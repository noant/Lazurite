using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.IOC;
using Lazurite.Scenarios.ScenarioTypes;
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
    public partial class ActionView : Grid, IConstructorElement
    {
        public ActionView()
        {
            InitializeComponent();
            
            buttons.EditClick += () => BeginEditAction();
            buttons.ChangeClick += () => BeginSelectAction();
        }

        public void MakeButtonsInvisible()
        {
            buttons.Visibility = Visibility.Collapsed;
        }

        public void MakeChangeButtonInvisible()
        {
            buttons.ChangeVisible = false;
        }

        public void BeginSelectAction()
        {
            ActionInstanceSide actionInstanceSide;
            if (this.AlgorithmContext is SingleActionScenario)
                actionInstanceSide = ActionInstanceSide.Both;
            else if (MasterAction == null)
                actionInstanceSide = ActionInstanceSide.OnlyLeft;
            else
                actionInstanceSide = ActionInstanceSide.OnlyRight;
            SelectActionView.Show(
                    (type) => {
                        var newAction = Singleton.Resolve<PluginsManager>().CreateInstanceOf(type, AlgorithmContext);
                        if (newAction != null)
                        {
                            ActionControlResolver.UserInitialize(
                                (result) => {
                                    if (result)
                                    {
                                        ActionHolder.Action = newAction;
                                        Model.Refresh(ActionHolder);
                                        Modified?.Invoke(this);

                                        if (MasterAction != null && !MasterAction.ValueType.IsCompatibleWith(newAction.ValueType))
                                        {
                                            MessageView.ShowMessage(
                                                "Тип действия не совпадает с типом действия главного действия. Нужно настроить подчиненное действие еще раз.",
                                                "Внимание!",
                                                Icons.Icon.WarningCircle);
                                        }
                                    }
                                },
                                newAction,
                                MasterAction?.ValueType,
                                true,
                                MasterAction);
                        }
                    },
                    MasterAction?.ValueType.GetType(),
                    actionInstanceSide,
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
