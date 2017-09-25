using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Icons;
using System.Windows.Forms;

namespace MultimediaKeysPlugin
{
    [HumanFriendlyName("Эмуляция мультимедиа клавиши")]
    [SuitableValueTypes(typeof(StateValueType))]
    [LazuriteIcon(Icon.InputKeyboard)]
    [OnlyExecute]
    public class MultimediaKeysAction : IAction
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

        public event ValueChangedDelegate ValueChanged;

        public string LastValue { get; set; }

        public string GetValue(ExecutionContext context)
        {
            return LastValue;
        }

        public void Initialize()
        {
            var valueType = new StateValueType();
            valueType.AcceptedValues = KeysNames.Select(x => x.Key).ToArray();
            this.ValueType = valueType;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            var key = KeysNames[value];
            KeyPressWrapper.Press(key);
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            Initialize();
            return true;
        }

        public static readonly Dictionary<string, Keys> KeysNames = new Dictionary<string, Keys>() {
            { "Убавить звук", Keys.VolumeDown },
            { "Прибавить звук", Keys.VolumeUp },
            { "Убрать звук", Keys.VolumeMute },
            { "Следующий трек", Keys.MediaNextTrack },
            { "Предыдущий трек", Keys.MediaPreviousTrack },
            { "Стоп", Keys.MediaStop },
            { "Плей/пауза", Keys.MediaPlayPause },
        };
    }
}
