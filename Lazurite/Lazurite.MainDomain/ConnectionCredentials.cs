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
        
        public string GetAddress()
        {
            return string.Format("https://{0}:{1}/{2}", Host, Port, ServiceName);
        }

        public static ConnectionCredentials Default
        {
            get
            {
                return new ConnectionCredentials()
                {
                    Host = "localhost",
                    Port = 8080,
                    ServiceName = "lazurite"
                };
            }
        }
    }
}
