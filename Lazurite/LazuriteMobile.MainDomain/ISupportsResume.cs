using System;

namespace LazuriteMobile.MainDomain
{
    public interface ISupportsResume
    {
        SupportsResumeStateChanged StateChanged { get; set; }
    }

    public delegate void SupportsResumeStateChanged(ISupportsResume supportsResume, SupportsResumeState currentState, SupportsResumeState previousState);
}
