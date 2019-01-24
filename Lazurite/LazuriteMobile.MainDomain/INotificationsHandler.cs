namespace LazuriteMobile.MainDomain
{
    public interface INotificationsHandler
    {
        void UpdateNotificationsInfo();
        bool NeedViewPermanently { get; }
    }
}
