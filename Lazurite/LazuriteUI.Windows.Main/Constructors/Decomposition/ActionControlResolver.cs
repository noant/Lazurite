using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.CoreActions.StandardValueTypeActions;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Constructors.StandardActionsInitialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    public static class ActionControlResolver
    {
        public static UIElement Create(IAction action)
        {
            if (action is ExecuteAction)
                return new ExecuteActionView((ExecuteAction)action);
            throw new NotImplementedException();
        }

        public static void UserInitialize(Action<bool> callback, IAction action, ValueTypeBase valueType, bool inheritsSupportedValues, IAction masterAction)
        {
            if (Lazurite.ActionsDomain.Utils.IsCoreVisualInitialization(action.GetType()))
            {
                IStandardVTActionEditView vtAction = CreateControl((IStandardValueAction)action, masterAction);

                var dialog = new DialogView((FrameworkElement)vtAction);
                vtAction.ApplyClicked += () => {
                    callback(true);
                    dialog.Close();
                };
                dialog.Show();
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
