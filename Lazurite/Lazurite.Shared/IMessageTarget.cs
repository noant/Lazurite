namespace Lazurite.Shared
{
    public interface IMessageTarget
    {
        void SetMessage(string message, string title);
        Messages ExtractMessages();
        string Id { get; set; }
        string Name { get; set; }
    }
}