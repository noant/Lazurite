﻿using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Icons;
using Lazurite.ActionsDomain.Attributes;

namespace CommonPlugin
{
    [OnlyGetValue]
    [HumanFriendlyName("День месяца сейчас")]
    [SuitableValueTypes(typeof(FloatValueType))]
    [LazuriteIcon(Icon.CalendarDay)]
    public class DayNowAction : IAction
    {
        public string Caption
        {
            get;
            set;
        } = string.Empty;

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
        } = new FloatValueType() { AcceptedValues = new[] { "1", "31" } };

        public event ValueChangedDelegate ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            return DateTime.Now.Day.ToString();
        }

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //do nothing
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
            //do nothing
        }
    }
}