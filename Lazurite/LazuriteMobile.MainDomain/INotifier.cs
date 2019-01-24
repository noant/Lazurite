using Lazurite.Shared;

namespace LazuriteMobile.MainDomain
{
    public interface INotifier
    {
        void Notify(Message message);
        LazuriteNotification[] GetNotifications();
    }
}
