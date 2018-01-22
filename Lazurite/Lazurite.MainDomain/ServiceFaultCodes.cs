using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public static class ServiceFaultCodes
    {
        public const string AccessDenied = "403";
        public const string ObjectAccessDenied = "403.1";
        public const string ObjectNotFound = "404";
        public const string InternalError = "500";
        public const string DecryptionError = "Decryption error";
    }
}
