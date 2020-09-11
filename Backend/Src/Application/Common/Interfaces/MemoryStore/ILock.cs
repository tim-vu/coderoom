using System;

namespace Application.Common.Interfaces.MemoryStore
{
    public interface ILock : IDisposable
    {
        bool IsAcquired { get; }
    }
}