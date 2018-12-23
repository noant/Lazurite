using Lazurite.MainDomain;
using System;
using System.Collections.Generic;

namespace Lazurite.MainDomain
{
    public class BruteforceChecker
    {
        public static ushort LoginHoursWaitTime = GlobalSettings.Get<ushort>(2);
        public static ushort LoginTryLifetimeMinutes = GlobalSettings.Get<ushort>(10);
        public static ushort LoginTryCount = GlobalSettings.Get<ushort>(5);

        private Dictionary<string, LoginInfo> _loginInfos = new Dictionary<string, LoginInfo>();

        private LoginInfo PrepareLastTryObject(string ip)
        {
            if (_loginInfos.ContainsKey(ip))
                return _loginInfos[ip];
            else {
                var info = new LoginInfo();
                _loginInfos.Add(ip, info);
                return info;
            };
        }

        private bool CheckBruteforceInternal(LoginInfo info)
        {
            var timePassed = DateTime.Now - info.LastLoginTry;
            if ((!info.IsBrutforceSuspicion && timePassed.TotalMinutes > LoginTryLifetimeMinutes) ||
                (info.IsBrutforceSuspicion && timePassed.TotalHours > LoginHoursWaitTime))
            {
                info.TryCount = 0;
                info.IsBrutforceSuspicion = false;
            }
            else if (!info.IsBrutforceSuspicion && timePassed.TotalMinutes < LoginTryLifetimeMinutes && info.TryCount >= LoginTryCount)
            {
                info.IsBrutforceSuspicion = true;
            }
            else if (!info.IsBrutforceSuspicion && timePassed.TotalMinutes < LoginTryLifetimeMinutes && info.TryCount < LoginTryCount)
            {
                info.TryCount++;
            }
            info.LastLoginTry = DateTime.Now;
            return info.IsBrutforceSuspicion;
        }
        
        public bool CheckIsBruteforce(string ip)
        {
            var info = PrepareLastTryObject(ip);
            return CheckBruteforceInternal(info);
        }

        public bool IsWaitListContains(string ip)
        {
            var info = PrepareLastTryObject(ip);
            return info.IsBrutforceSuspicion && (DateTime.Now - info.LastLoginTry).TotalHours < LoginHoursWaitTime;
        }

        private class LoginInfo
        {
            public DateTime LastLoginTry { get; set; } = DateTime.MinValue;
            public int TryCount { get; set; } = 1;
            public bool IsBrutforceSuspicion { get; set; }
        }
    }
}