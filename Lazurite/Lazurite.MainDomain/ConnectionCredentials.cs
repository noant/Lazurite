using Lazurite.Data;
using System.Collections.Generic;

namespace Lazurite.MainDomain
{
    [EncryptFile]
    public struct ConnectionCredentials
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string SecretKey { get; set; }

        public string GetAddress() => string.Format("https://{0}:{1}", Host, Port);

        public override bool Equals(object obj)
        {
            return obj != null &&
                obj is ConnectionCredentials &&
                obj.GetHashCode().Equals(GetHashCode());
        }

        public override int GetHashCode()
        {
            var hashCode = -1366384121;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Host);
            hashCode = hashCode * -1521134295 + Port.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Login);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Password);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SecretKey);
            return hashCode;
        }

        public static readonly ConnectionCredentials Default = new ConnectionCredentials()
        {
            Host = "localhost",
            Port = 8080
        };
    }
}
