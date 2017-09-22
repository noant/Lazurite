using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LazuriteUI.Icons
{
    public class LazuriteIconAttribute: Attribute
    {
        public Icon Icon { get; private set; }
        
        public LazuriteIconAttribute(Icon icon)
        {
            Icon = icon;
        }

        public static Icon GetIcon(Type type)
        {
            var attribute = type.GetTypeInfo().GetCustomAttribute<LazuriteIconAttribute>();
            if (attribute != null)
                return attribute.Icon;
            else return Icon._None;
        }
    }
}
