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
    [LazuriteIcon(Icon.Sound3)]
    [HumanFriendlyName("Звуковая нотификация")]
    [Category(Category.Other)]
    [SuitableValueTypes(typeof(StateValueType))]
    public class SoundNotify : IAction
    {
        public string Caption { get; set; } = "Звуковая нотификация";

        public ValueTypeBase ValueType { get; set; } = new StateValueType() {
            AcceptedValues = new[] { "ОК", "Принять", "Отмена" }
        };

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => false;

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context) => string.Empty;

        public void Initialize() { }

        public void SetValue(ExecutionContext context, string value) {
            switch (value)
            {
                case "ОК": NotificationWindow.SoundNotify(); return;
                case "Принять": NotificationWindow.SoundNotifyAccept(); return;
                case "Отмена": NotificationWindow.SoundNotifyDisallow(); return;
            }
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues) => true;
    }
}
