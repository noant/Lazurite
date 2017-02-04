using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    public class VisualSettingsBase
    {
        public string ScenarioId { get; set; } //guid

        public byte[] Color { get; set; } // 3 bytes

        public int PositionX { get; set; }

        public int PositionY { get; set; }
    }
}
