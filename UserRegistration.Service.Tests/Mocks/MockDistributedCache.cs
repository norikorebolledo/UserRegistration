using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UserRegistration.Service.Tests.Mocks
{
    public class MockDistributedCache : IDistributedCache
    {
        private static ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();
        public byte[] Get(string key)
        {
            if (_data.ContainsKey(key))
                return Encoding.UTF8.GetBytes(_data[key]);

            return null;
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            return Task.Run(() => Get(key));
        }

        public void Refresh(string key)
        {
            throw new NotImplementedException();
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            _data.AddOrUpdate(key, Encoding.UTF8.GetString(value), (k, v) => v);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            return Task.Run(() => Set(key, value, options));
        }
    }
}
