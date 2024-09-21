﻿using Core.Result.AppResults;
using Destructurama.Attributed;
using MediatR;

namespace Identity.Application.Services
{
    public class LoginRequest(string UserName, string Password) : IRequest<AppResult<string>>
    {
        public string UserName { get; set; } = UserName;
        [NotLogged]
        public string Password { get; set; } = Password;
    }
}
