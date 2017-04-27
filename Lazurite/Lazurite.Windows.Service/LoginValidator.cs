using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Windows.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Web;

namespace Lazurite.Windows.Service
{
    public class LoginValidator : UserNamePasswordValidator
    {
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly LoggerBase Log = Singleton.Resolve<LoggerBase>();

        public override void Validate(string userName, string password)
        {
            try
            {
                var passwordHash = CryptoUtils.CreatePasswordHash(password);
                var user = UsersRepository.Users.SingleOrDefault(x => x.Login.Equals(userName) && x.PasswordHash.Equals(passwordHash));
                if (user == null)
                    throw new FaultException("Login or password not valid", new FaultCode("403"));
            }
            catch (FaultException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                Log.Write(e, "Error while user authenticate: " + userName);
                throw new FaultException("Error while user authenticate: " + userName, new FaultCode("500"));
            }
        }
    }
}