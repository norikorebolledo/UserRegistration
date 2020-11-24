using Core.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Models;
using UserRegistration.Service.Contracts;

namespace UserRegistration.Api.Areas.Backend.WS
{
    public class AccountWebSocket : WebSocketHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public AccountWebSocket(WebSocketConnectionManager webSocketConnectionManager, IServiceScopeFactory serviceScopeFactory)
            : base(webSocketConnectionManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var model = JsonConvert.DeserializeObject<UserRegistrationRequest>(message);

            if (model.Command == "register")
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var authService = scope.ServiceProvider.GetService<IAuthService>();
                    var response = await authService.Register(model);
                    await SendMessageAsync(socketId, JsonConvert.SerializeObject(response));
                }
                
                
            }
        }
    }
}
