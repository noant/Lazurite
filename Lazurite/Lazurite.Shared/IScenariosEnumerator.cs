using System;

namespace Lazurite.Shared
{
    public interface IScenariosEnumerator
    {
        void SetCasts(Func<ScenarioCast[]> needCasts);
    }
}
