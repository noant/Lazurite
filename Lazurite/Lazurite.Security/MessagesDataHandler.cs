using Lazurite.MainDomain;
using Lazurite.Shared;

namespace Lazurite.Security
{
    public class MessagesDataHandler : IAddictionalDataHandler
    {
        public void Handle(AddictionalData data, object tag)
        {
            //
        }

        public void Initialize()
        {
            //
        }

        public void Prepare(AddictionalData data, object tag)
        {
            var user = tag as IMessageTarget;
            if (user != null)
                data.Set(user.ExtractMessages());
        }
    }
}
