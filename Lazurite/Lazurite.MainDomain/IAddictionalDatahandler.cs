using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public interface IAddictionalDataHandler
    {
        void Handle(AddictionalData data);
        void Prepare(AddictionalData data);
        void Initialize();
    }
}
