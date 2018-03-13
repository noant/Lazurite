using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public class ScenarioActionSource
    {
        public ScenarioActionSource(UserBase user, ScenarioStartupSource source, ScenarioAction action)
        {
            User = user;
            Source = source;
            Action = action;
        }

        public UserBase User { get; }
        public ScenarioStartupSource Source { get; }
        public ScenarioAction Action { get; }
    }
}
