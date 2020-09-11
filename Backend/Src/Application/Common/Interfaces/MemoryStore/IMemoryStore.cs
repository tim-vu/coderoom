using System;
using System.Threading.Tasks;
using Application.Common.Interfaces.MemoryStore;

namespace Application.Common.Interfaces
{
    public interface IMemoryStore
    {
        Task<bool> KeyExists(string key);
        Task<ILock> CreateLock(string key);
        
        Task<T> ObjectGet<T>(string key);

        Task ObjectSet<T>(string key, T @object);

        Task Expire(string key, TimeSpan timeSpan);
    }
}