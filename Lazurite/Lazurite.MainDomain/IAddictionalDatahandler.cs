namespace Lazurite.MainDomain
{
    public interface IAddictionalDataHandler
    {
        void Handle(AddictionalData data, object tag);
        void Prepare(AddictionalData data, object tag);
        void Initialize();
    }
}
