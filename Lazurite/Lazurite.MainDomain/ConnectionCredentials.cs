namespace Lazurite.MainDomain
{
    public struct ConnectionCredentials
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public string ServiceName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string SecretKey { get; set; }
        
        public override int GetHashCode()
        {
            return Host.GetHashCode() ^ Port.GetHashCode() ^
                ServiceName.GetHashCode() ^ Login.GetHashCode() ^
                Password.GetHashCode() ^ SecretKey.GetHashCode();
        }

        public string GetAddress() => string.Format("https://{0}:{1}/{2}", Host, Port, ServiceName);

        public static readonly ConnectionCredentials Default = new ConnectionCredentials()
        {
            Host = "localhost",
            Port = 8080,
            ServiceName = "lazurite"
        };
    }
}
