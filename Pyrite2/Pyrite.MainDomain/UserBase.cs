using Pyrite.Data;
using Pyrite.IOC;

namespace Pyrite.MainDomain
{
    public abstract class UserBase
    {
        public readonly ISavior Savior = Singleton.Resolve<ISavior>();

        public string Id { get; set; } //guid

        public void Save()
        {
            Savior.Set(this.Id, this);
        }
    }
}