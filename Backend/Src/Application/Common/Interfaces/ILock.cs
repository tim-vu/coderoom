using System;

namespace Application.Common.Interfaces
{
    public interface ILock : IDisposable
    {
        bool IsAcquired { get; }
    }
}