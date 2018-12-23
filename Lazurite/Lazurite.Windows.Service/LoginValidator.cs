using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Utils;
using System;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Net;
using System.ServiceModel;

namespace Lazurite.Windows.Service
{
    public class LoginValidator : UserNamePasswordValidator
    {
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly WarningHandlerBase WarningHandler = Singleton.Resolve<WarningHandlerBase>();
        
        public override void Validate(string userName, string password)
        {
            try
            {
                WarningHandler.Debug("Authentication try: " + userName);
                var passwordHash = CryptoUtils.CreatePasswordHash(password);
                var user = UsersRepository.Users.SingleOrDefault(x => x.Login.Equals(userName) && x.PasswordHash.Equals(passwordHash));
                if (user == null)
                    Throw(ServiceFaultCodes.AccessDenied);
                WarningHandler.Debug("Authentication success: " + userName);
            }
            catch (FaultException e)
            {
                WarningHandler.Debug("Authentication failed: " + userName);
                throw e;
            }
            catch (Exception e)
            {
                WarningHandler.InfoFormat(e, "Error while user authenticate: [{0}]", userName);
                Throw(ServiceFaultCodes.InternalError);
            }
        }

        private void Throw(string code)
        {
            throw new FaultException("Error: " + code, new FaultCode(code));
        }
    }
}