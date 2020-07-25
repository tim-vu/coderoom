using Application.Common.Interfaces;
using RedLockNet;

namespace Infrastructure.Redis
{
    public class RedisLock : ILock
    {
        public bool IsAcquired => _lock.IsAcquired;
        
        private readonly IRedLock _lock;

        public RedisLock(IRedLock @lock)
        {
            _lock = @lock;
        }

        public void Dispose()
        {
            _lock.Dispose();
        }
    }
}