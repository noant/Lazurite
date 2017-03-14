using Pyrite.IOC;
using Pyrite.MainDomain;
using Pyrite.Windows.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Web;

namespace Pyrite.Windows.Service
{
    public class LoginValidator : UserNamePasswordValidator
    {
        //private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        //private static readonly LoggerBase Log = Singleton.Resolve<LoggerBase>();

        public override void Validate(string userName, string password)
        {
            //try
            //{
            //    var passwordHash = CryptoUtils.CreatePasswordHash(password);
            //    var user = UsersRepository.Users.SingleOrDefault(x => x.Equals(userName) && x.PasswordHash.Equals(passwordHash));
            //    if (user == null)
            //        throw new SecurityAccessDeniedException("Login or password not valid", new FaultException("Login or password not valid"));
            //}
            //catch (SecurityAccessDeniedException e)
            //{
            //    throw e;
            //}
            //catch (Exception e)
            //{
            //    Log.Write(e, "Error while user authenticate: " + userName);
            //    throw e;
            //}
        }
    }
}