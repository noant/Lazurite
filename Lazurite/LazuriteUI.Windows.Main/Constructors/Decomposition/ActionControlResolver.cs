using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.CoreActions.StandardValueTypeActions;
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
                IStandardVTActionEditView vtAction = null;
                if (action is GetFloatVTAction)
                    vtAction = new FloatInitializationView((IStandardValueAction)action, masterAction);
                else if (action is GetToggleVTAction)
                    vtAction = new ToggleInitializationView((IStandardValueAction)action, masterAction);
                else if (action is GetDateTimeVTAction)
                    vtAction = new DateTimeInitializationView((IStandardValueAction)action, masterAction);
                else if (action is GetStateVTAction)
                    vtAction = new StatusInitializationView((IStandardValueAction)action, masterAction);
                else if (action is GetInfoVTAction)
                    vtAction = new InfoInitializationView((IStandardValueAction)action, masterAction);

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
    }
}
