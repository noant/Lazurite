﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Icons
{
    public class LazuriteIconAttribute: Attribute
    {
        public Icon Icon { get; private set; }

        public Stream Data { get; private set; }
        
        public LazuriteIconAttribute(Icon icon)
        {
            Icon = icon;
            Data = Utils.GetIconData(icon);
        }
    }
}
