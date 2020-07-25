using System;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using RedLockNet;
using StackExchange.Redis;

namespace Infrastructure.Redis
{
    public class RedisMemoryStore : IMemoryStore
    {
        private static readonly TimeSpan LockExpiry = TimeSpan.FromMilliseconds(500);
        private static readonly TimeSpan LockRetry = TimeSpan.FromMilliseconds(50);
        private static readonly TimeSpan LockWait = TimeSpan.FromSeconds(2);
        
        private readonly IConnectionMultiplexer _connection;
        private readonly IDistributedLockFactory _lockFactory;

        public RedisMemoryStore(IConnectionMultiplexer connection, IDistributedLockFactory lockFactory)
        {
            _connection = connection;
            _lockFactory = lockFactory;
        }

        public Task<ILock> CreateLock(string key)
        {
            return _lockFactory.CreateLockAsync(key, LockExpiry, LockRetry, LockWait).ContinueWith(t => (ILock)new RedisLock(t.Result));
        }

        public Task ObjectSet<T>(string key, T t)
        {
            var value = JsonSerializer.Serialize(t);
            var db = _connection.GetDatabase();
            return db.StringSetAsync(key, value);
        }

        public Task Expire(string key, TimeSpan timeSpan)
        {
            var db = _connection.GetDatabase();
            return db.KeyExpireAsync(key, timeSpan);
        }

        public Task<T> ObjectGet<T>(string key)
        {
            var db = _connection.GetDatabase();
            return db.StringGetAsync(key).ContinueWith(t =>
            {
                var value = t.Result;
                return value.IsNull ? default : JsonSerializer.Deserialize<T>(value.ToString());
            });
        }
        public Task<string> StringGet(string key)
        {
            var db = _connection.GetDatabase();
            return db.StringGetAsync(key).ContinueWith(t => t.Result.ToString());
        }

        public Task StringSet(string key, string value)
        {
            var db = _connection.GetDatabase();
            return db.StringSetAsync(key, value);
        }
    }
}