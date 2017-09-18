using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NModbusWrapper
{
    public interface IModbusSingleCoilAction
    {
        NModbusManager Manager { get; set; }

        byte SlaveAddress { get; set; }

        byte CoilAddress { get; set; }
    }
}
