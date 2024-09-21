﻿using Core.AspNet.Result;
using Core.Result.Paginations;
using EmployeeMgt.Application.Services;
using EmployeeMgt.Domain.Entities;
using FastEndpoints;
using MediatR;

namespace EmployeeMgt.API.Endpoint
{
    public class GetEmployeeEndpoint : Endpoint<GetEmployeeRequest, HttpResultTyped<PagingResponse<Employee>>>
    {
        private readonly IMediator _mediator;
        public GetEmployeeEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override void Configure()
        {
            Get("employee/get");
        }

        public override async Task HandleAsync(GetEmployeeRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            await SendResultAsync(result.ToHttpResult());
        }
    }
}
