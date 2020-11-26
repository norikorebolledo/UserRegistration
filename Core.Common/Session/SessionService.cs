using Core.Common.Contracts.Session;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Session
{
    public class SessionService : ISessionService
    {
        private readonly IDistributedCache _distributedCache;
        public SessionService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<string> GetAsync(string sessionId)
        {
            return await _distributedCache.GetStringAsync(sessionId);
        }

        public async Task RefreshAsync(string sessionId)
        {
            await _distributedCache.RefreshAsync(sessionId);
        }

        public async Task SetAsync(string sessionId, string data, int expirationInSeconds)
        {
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(expirationInSeconds)
            };

            await _distributedCache.SetAsync(sessionId, Encoding.UTF8.GetBytes(data), options);
        }

        public async Task RemoveAsync(string sessionId)
        {
            await _distributedCache.RemoveAsync(sessionId);
        }
    }
}
