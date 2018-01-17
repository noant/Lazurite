using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MultimediaKeysPlugin
{
    [HumanFriendlyName("Эмуляция клавиши")]
    [SuitableValueTypes(typeof(StateValueType))]
    [LazuriteIcon(Icon.InputKeyboard)]
    [OnlyExecute]
    [Category(Category.Multimedia)]
    public class KeysAction : IAction
    {
        public string Caption
        {
            get
            {
                return string.Empty;
            }

            set
            {
                //do nothing
            }
        }

        public bool IsSupportsEvent
        {
            get
            {
                return false;
            }
        }

        public bool IsSupportsModification
        {
            get
            {
                return false;
            }
        }

        public ValueTypeBase ValueType
        {
            get;
            set;
        }

        public event ValueChangedEventHandler ValueChanged;

        public string LastValue { get; set; }

        public string GetValue(ExecutionContext context)
        {
            return LastValue;
        }

        public void Initialize()
        {
            if (!CachedKeys.Any())
            {
                var allKeys = Enum.GetValues(typeof(Keys));
                foreach (var key in allKeys)
                {
                    var name = Enum.GetName(typeof(Keys), key);
                    //hardcode begin
                    if (CachedKeys.ContainsKey(name))
                        name = name + " (2)";
                    if (CachedKeys.ContainsKey(name))
                        name = name.Replace("(2)", "(3)");
                    if (CachedKeys.ContainsKey(name))
                        name = name.Replace("(3)", "(4)");
                    if (CachedKeys.ContainsKey(name))
                        name = name.Replace("(4)", "(5)");
                    if (CachedKeys.ContainsKey(name))
                        name = name.Replace("(5)", "(6)");
                    //hardcode end
                    CachedKeys.Add(name, (Keys)key);
                }
            }
                       
            var valueType = new StateValueType();
            valueType.AcceptedValues = CachedKeys.Select(x=>x.Key).ToArray();
            this.ValueType = valueType;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            var key = CachedKeys[value];
            KeyPressWrapper.Press(key);
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            Initialize();
            return true;
        }

        private static Dictionary<string, Keys> CachedKeys = new Dictionary<string, Keys>();
    }
}
