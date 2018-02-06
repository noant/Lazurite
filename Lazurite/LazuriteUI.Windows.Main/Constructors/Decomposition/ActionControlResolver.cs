using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.CoreActions.ContextInitialization;
using Lazurite.CoreActions.CoreActions;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Constructors.StandardActionsInitialization;
using System;
using System.Windows;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    public static class ActionControlResolver
    {
        public static IConstructorElement Create(ActionHolder actionHolder, IAlgorithmContext algoContext)
        {
            IConstructorElement element;
            if (actionHolder.Action is ExecuteAction)
                element = new ExecuteActionView();
            else if (actionHolder.Action is SetReturnValueAction)
                element = new ReturnScenarioValueView();
            else if (actionHolder.Action is IfAction)
                element = new IfActionView();
            else if (actionHolder.Action is WhileAction)
                element = new WhileActionView();
            else if (actionHolder.Action is CancelExecutionAction)
                element = new CancelExecutionActionView();
            else
                throw new NotImplementedException();
            element.Refresh(actionHolder, algoContext);
            return element;
        }

        public static void UserInitialize(Action<bool> callback, IAction action, ValueTypeBase valueType, bool inheritsSupportedValues, IAction masterAction)
        {
            if (action is IStandardValueAction)
            {
                IStandardVTActionEditView vtAction = CreateControl((IStandardValueAction)action, masterAction);
                if (vtAction == null)
                    callback?.Invoke(true);
                var dialog = new DialogView((FrameworkElement)vtAction);
                vtAction.ApplyClicked += () =>
                {
                    callback(true);
                    dialog.Close();
                };
                dialog.Show();
            }
            else if (action is RunExistingScenarioAction)
            {
                var runExistingScenarioAction = (RunExistingScenarioAction)action;
                SelectScenarioAndRunModeView.Show(
                    (selectedScenario, selectedMode) => {
                        var id = selectedScenario.Id;
                        runExistingScenarioAction.Mode = selectedMode;
                        runExistingScenarioAction.TargetScenarioId = id;
                        runExistingScenarioAction.SetTargetScenario(selectedScenario);
                        callback(true);
                    },
                    valueType?.GetType(),
                    Lazurite.Windows.Modules.ActionInstanceSide.OnlyLeft,
                    runExistingScenarioAction.TargetScenarioId,
                    runExistingScenarioAction.Mode);
            }
            else if (action is GetExistingScenarioValueAction)
            {
                var getScenarioValueAction = (GetExistingScenarioValueAction)action;
                SelectScenarioView.Show(
                    (selectedScenario) => {
                        var id = selectedScenario.Id;
                        getScenarioValueAction.TargetScenarioId = id;
                        getScenarioValueAction.SetTargetScenario(selectedScenario);
                        callback(true);
                    },
                    valueType?.GetType(),
                    Lazurite.Windows.Modules.ActionInstanceSide.OnlyRight,
                    getScenarioValueAction.TargetScenarioId);
            }
            else
                callback(action.UserInitializeWith(valueType, inheritsSupportedValues));
        }

        public static void BeginCompositeScenarioSettings(CompositeScenario compositeScenario, Action<bool> callback)
        {
            var control = CreateControl(compositeScenario);
            var dialog = new DialogView((FrameworkElement)control);
            if (compositeScenario.ValueType is StateValueType)
                dialog.Caption =
                    "В данном окне можно настроить значение по умолчанию и статусы, " +
                    "которые может принимать и возвращать данный сценарий. " +
                    "Для того, чтобы добавить статус, нужно ввести в поле ввода " +
                    "его название и нажать \"+\". Для выбора значения сценария по " +
                    "умолчанию, нужно выбрать любой статус из списка.";
            else if (compositeScenario.ValueType is FloatValueType)
                dialog.Caption =
                    "Выберите числовое значение, которое сценарий будет принимать при инициализации. Настройте максимальное и минимальное значение, которое можно задать сценарию.";
            else
                dialog.Caption =
                    "Выберите значение, которое сценарий будет принимать при инициализации.";

            bool ignoreCloseEvent = false;

            control.ApplyClicked += () =>
            {
                ignoreCloseEvent = true;
                callback?.Invoke(true);
                dialog.Close();
            };
            dialog.Closed += (o, e) => {
                if (!ignoreCloseEvent)
                    callback?.Invoke(false);
            };
            dialog.Show();
        }

        public static IStandardVTActionEditView CreateControl(IStandardValueAction action, IAction masterAction = null)
        {
            IStandardVTActionEditView vtAction = null;
            if (action.ValueType is FloatValueType)
                vtAction = new FloatInitializationView(action, masterAction);
            else if (action.ValueType is ToggleValueType)
                vtAction = new ToggleInitializationView(action, masterAction);
            else if (action.ValueType is DateTimeValueType)
                vtAction = new DateTimeInitializationView(action, masterAction);
            else if (action.ValueType is StateValueType)
                vtAction = new StatusInitializationView(action, masterAction);
            else if (action.ValueType is InfoValueType)
                vtAction = new InfoInitializationView(action, masterAction);
            return vtAction;
        }
    }
}
