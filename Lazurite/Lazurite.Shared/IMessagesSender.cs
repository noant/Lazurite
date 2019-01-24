using System;

namespace Lazurite.Shared
{
    public interface IMessagesSender
    {
        void SetNeedTargets(Func<IMessageTarget[]> needTargets);
    }
}