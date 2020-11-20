using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserRegistration.Models;
using UserRegistration.Service.Contracts;

namespace UserRegistration.Api.Areas.Backend.Hubs
{
    public class AccountHub : Hub
    {
        private readonly IAccountService _accountService;

        public AccountHub(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task Register(UserRegistrationRequest model)
        {
            var response = _accountService.Register(model);

            await Clients.Client(Context.ConnectionId).SendAsync("registerResponse", response);
        }
    }
}
