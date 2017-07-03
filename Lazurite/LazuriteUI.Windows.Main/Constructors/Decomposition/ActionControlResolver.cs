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
                    vtAction = new FloatInitializationView(action, masterAction);
                else
                    throw new NotImplementedException();

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
