﻿using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.MainDomain
{
    public interface IGeolocationView
    {
        void View(Geolocation geolocation, string label);
    }
}
