using Core.WebSocket;
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
        //private readonly IAccountService _accountService;
        public AccountWebSocket(WebSocketConnectionManager webSocketConnectionManager) //, IAccountService accountService) 
            : base(webSocketConnectionManager)
        {
           // _accountService = accountService;
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetId(socket);
            await SendMessageToAllAsync($"{socketId} is now connected");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var model = JsonConvert.DeserializeObject<UserRegistrationRequest>(message);

            //var response = _accountService.Register(model);

            await SendMessageAsync(socketId, message);
        }
    }
}
