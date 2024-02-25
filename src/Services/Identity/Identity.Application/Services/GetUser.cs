﻿using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using Kernel.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Kernel.Dependency;

namespace Identity.Application.Services
{
    public class GetUser : IRequestHandler<GetUserRequest, AppResult<PagingResponse<User>>>, ITransient
    {
        private readonly IAppDbContext _context;
        public GetUser(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<AppResult<PagingResponse<User>>> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            var pageUser = PagingTyped.PagingResult(await _context.Users.ToListAsync(), request);
            return AppResult.Success(pageUser);
		}
    }
}