using System;

namespace LazuriteMobile.MainDomain
{
    public interface ISupportsResume
    {
        SupportsResumeResumed OnResume { get; set; }
    }

    public delegate void SupportsResumeResumed(ISupportsResume supportsResume, SupportsResumeState previousState);
}
