using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Contracts.Session
{
    public interface ISessionService
    {
        Task<string> GetAsync(string sessionId);
        Task SetAsync(string sessionId, string data, int expirationInSeconds);
        Task RefreshAsync(string sessionId);
        Task RemoveAsync(string sessionId);
    }
}
