namespace Lazurite.Windows.Modules
{
    public class CanUpdatePluginResult
    {
        public CanUpdatePluginResult(bool can, string message = null)
        {
            CanUpdate = can;
            Message = message;
        }
        public string Message { get; private set; }
        public bool CanUpdate { get; private set; }
    }
}
