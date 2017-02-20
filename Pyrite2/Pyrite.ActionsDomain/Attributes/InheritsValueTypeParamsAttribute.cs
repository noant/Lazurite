using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.Attributes
{
    /// <summary>
    /// Действия с этим аттрибутом наследуют параметры ValueType при том же типе 
    /// ValueType, могут быть использованы только в правой части выражения
    /// </summary>
    public class InheritsValueTypeParamsAttribute : Attribute
    {
    }
}
