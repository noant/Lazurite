using Lazurite.ActionsDomain;
using Lazurite.CoreActions.StandardValueTypeActions;
using LazuriteUI.Windows.Main.Switches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Constructors.StandardActionsInitialization
{
    public class DateTimeInitializationView : DateTimeViewSwitch, IStandardVTActionEditView
    {
        public DateTimeInitializationView(IStandardValueAction action = null, IAction masterAction = null)
        {
            var dateTime = DateTime.Now;
            DateTime.TryParse(action.Value, out dateTime);
            if (dateTime == DateTime.MinValue)
                dateTime = DateTime.Now;//crutch
            this.DateTime = dateTime;

            this.Apply += (o, e) =>
            {
                action.Value = this.DateTime.ToString();
                ApplyClicked?.Invoke();
            };
        }

        public event Action ApplyClicked;
    }
}
