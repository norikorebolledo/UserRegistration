using Core.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
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
            dynamic model = JsonConvert.DeserializeObject<ExpandoObject>(message);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                if (model.command == "register")
                {
                    var service = scope.ServiceProvider.GetService<IAuthService>();
                    var response = await service.Register(JsonConvert.DeserializeObject<UserRegistrationRequest>(message));
                    await SendMessageAsync(socketId, JsonConvert.SerializeObject(response));
                }
                else if (model.command == "loginSalt")
                {
                    var service = scope.ServiceProvider.GetService<ILoginService>();
                    var response = await service.GenerateSalt(JsonConvert.DeserializeObject<LoginSaltRequest>(message));
                    await SendMessageAsync(socketId, JsonConvert.SerializeObject(response));
                }
                else if (model.command == "login")
                {
                    var service = scope.ServiceProvider.GetService<ILoginService>();
                    var response = await service.Login(JsonConvert.DeserializeObject<LoginRequest>(message));
                    await SendMessageAsync(socketId, JsonConvert.SerializeObject(response));
                }
                else if (model.command == "register")
                {
                    var service = scope.ServiceProvider.GetService<IAuthService>();
                    var response = await service.Register(JsonConvert.DeserializeObject<UserRegistrationRequest>(message));
                    await SendMessageAsync(socketId, JsonConvert.SerializeObject(response));
                }
                else if (model.command == "emailVerification")
                {
                    var service = scope.ServiceProvider.GetService<IAuthService>();
                    var response = await service.SendVerificationCode(JsonConvert.DeserializeObject<VerificationRequest>(message));
                    await SendMessageAsync(socketId, JsonConvert.SerializeObject(response));
                }
                else if (model.command == "verification")
                {
                    var service = scope.ServiceProvider.GetService<IAuthService>();
                    var response = await service.SendVerificationCode(JsonConvert.DeserializeObject<VerificationRequest>(message));
                    await SendMessageAsync(socketId, JsonConvert.SerializeObject(response));
                }
                else if (model.command == "checkUsername")
                {
                    var service = scope.ServiceProvider.GetService<IAuthService>();
                    var response = await service.CheckUsername(JsonConvert.DeserializeObject<CheckUsernameAvailabilityRequest>(message));
                    await SendMessageAsync(socketId, JsonConvert.SerializeObject(response));
                }
                else if (model.command == "checkEmail")
                {
                    var service = scope.ServiceProvider.GetService<IAuthService>();
                    var response = await service.CheckEmail(JsonConvert.DeserializeObject<CheckEmailAvailabilityRequest>(message));
                    await SendMessageAsync(socketId, JsonConvert.SerializeObject(response));
                }
            }

        }


    }
}
