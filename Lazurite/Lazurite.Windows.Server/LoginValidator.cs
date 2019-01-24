using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Utils;
using SimpleRemoteMethods.ServerSide;
using System;
using System.Linq;

namespace Lazurite.Windows.Server
{
    public class LoginValidator : IAuthenticationValidator
    {
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly WarningHandlerBase WarningHandler = Singleton.Resolve<WarningHandlerBase>();

        public bool Authenticate(string userName, string password)
        {
            try
            {
                WarningHandler.Debug("Authentication try: " + userName);
                var passwordHash = CryptoUtils.CreatePasswordHash(password);
                var user = UsersRepository.Users.SingleOrDefault(x => x.Login.Equals(userName) && x.PasswordHash.Equals(passwordHash));
                if (user == null)
                {
                    WarningHandler.Debug("Authentication failed: " + userName);
                    return false;
                }
                WarningHandler.Debug("Authentication success: " + userName);
                return true;
            }
            catch (Exception e)
            {
                WarningHandler.Error("Error while user authentication: " + userName, e);
                return false;
            }
        }
    }
}