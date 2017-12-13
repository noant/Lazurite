using System;

namespace LazuriteMobile.MainDomain
{
    public interface ISupportsResume
    {
        Action<ISupportsResume> OnResume { get; set; }
    }
}
