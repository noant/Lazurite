using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using NotificationUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernNotifyPlugin
{
    [OnlyExecute]
    [LazuriteIcon(Icon.MessageSmiley)]
    [HumanFriendlyName("Нотификация")]
    [Category(Category.Other)]
    [SuitableValueTypes(typeof(InfoValueType))]
    public class Notify : IAction
    {
        private NotificationWindow _notificationWindow = new NotificationWindow();

        public string Caption { get; set; } = "Нотификация";

        public ValueTypeBase ValueType { get; set; } = new InfoValueType();

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => false;

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context) => string.Empty;

        public void Initialize() => _notificationWindow.Show();

        public void SetValue(ExecutionContext context, string value)
        {
            if (value.StartsWith("#"))
                NotifyInternal(value.Substring(1), true);
            else NotifyInternal(value, false);
        }

        private void NotifyInternal(string text, bool spec)
        {
            _notificationWindow.Dispatcher.BeginInvoke(new Action(() => {
                _notificationWindow.Notify(text, spec);
            }));
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues) => true;
    }
}
