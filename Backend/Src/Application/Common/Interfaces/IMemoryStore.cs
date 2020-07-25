using System;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IMemoryStore
    {
        Task<ILock> CreateLock(string key);
        
        Task<T> ObjectGet<T>(string key);

        Task ObjectSet<T>(string key, T @object);

        Task Expire(string key, TimeSpan timeSpan);
    }
}