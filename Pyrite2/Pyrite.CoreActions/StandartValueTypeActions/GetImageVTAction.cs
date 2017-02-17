using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions.StandartValueTypeActions
{
    [OnlyGetValue]
    [VisualInitialization]
    [HumanFriendlyName("Изображение")]
    public class GetImageVTAction : IAction
    {
        public string Caption
        {
            get
            {
                return "Изображение";
            }
            set
            {
                //
            }
        }

        public string Value
        {
            get;
            set;
        }

        private ToggleValueType _valueType = new ToggleValueType();
        public ActionsDomain.ValueTypes.AbstractValueType ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                //
            }
        }

        public void Initialize()
        {
            //
        }

        public void UserInitialize()
        {
            //
        }
    }
}
