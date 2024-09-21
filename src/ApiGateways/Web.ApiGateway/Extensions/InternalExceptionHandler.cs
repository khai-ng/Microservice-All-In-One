﻿using Core.AspNet.Result;
using Core.Autofac;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace Web.ApiGateway.Extensions
{
    public class InternalExceptionHandler : IExceptionHandler, ISingleton
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
        {
            await httpContext.Response.WriteAsJsonAsync(
                new HttpErrorResult(HttpStatusCode.InternalServerError,
                    exception.Message)
            );
            return true;
        }
    }
}
