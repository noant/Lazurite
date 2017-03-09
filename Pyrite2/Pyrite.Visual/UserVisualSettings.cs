using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pyrite.Visual
{
    public class UserVisualSettings: VisualSettingsBase
    {
        public string UserId { get; set; }

        public override bool SameAs(VisualSettingsBase settings)
        {
            return base.SameAs(settings)
                && ((UserVisualSettings)settings).UserId.Equals(UserId);
        }
    }
}
