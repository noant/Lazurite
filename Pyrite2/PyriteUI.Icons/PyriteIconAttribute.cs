using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyriteUI.Icons
{
    public class PyriteIconAttribute: Attribute
    {
        public Icon Icon { get; private set; }

        public Stream Data { get; private set; }
        
        public PyriteIconAttribute(Icon icon)
        {
            Icon = icon;
            Data = Utils.GetIconData(icon);
        }
    }
}
