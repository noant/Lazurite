using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lazurite.CoreActions
{
    public class ActionHolder
    {
        public IAction Action { get; set; } = new EmptyAction();
    }
}
