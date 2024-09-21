﻿using Core.Result.AppResults;
using MediatR;

namespace Identity.Application.Services
{
    public record SignInRequest(string UserName, string Password) : IRequest<AppResult<string>>
    { }
}
