﻿using Core.Autofac;
using Core.EntityFramework.Pagination;
using Core.EntityFramework.Repository;
using Core.Pagination;
using Identity.API.Application.Common.Abstractions;
using Identity.Application.Services;
using Identity.Domain.Entities.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository, ITransient
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> FindAsync(string userName)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<PagingResponse<User>> GetPagedAsync(GetUserRequest request)
        {
            return await FluentPaging
                .From(request)
                .PagingAsync(_context.Users);
        }

        public async Task<IEnumerable<string>> GetListRoleAsync(Guid userId)
        {
            return await _context.Users
                .Include(x => x.Name)
                .Include(x => x.Roles)
                .Where(x => x.Id == userId)
                .SelectMany(x => x.Roles)
                .Select(x => x.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetListPermissionAsync(Guid userId)
        {
            IEnumerable<Permission> rolePermissions = await _context.Users
               .Include(x => x.Roles)
               .ThenInclude(x => x.Permissions)    
               .Where(x => x.Id == userId)
               .SelectMany(x => x.Roles)
               .SelectMany(x => x.Permissions)
               .ToListAsync();

            IEnumerable<Permission> permissions = await _context.Users
                .Include(x => x.Permissions)
                .Where(x => x.Id == userId)
                .SelectMany(x => x.Permissions)
                .ToListAsync();

            var allPermissions = rolePermissions.Union(permissions);

            return allPermissions.Select(x => x.Name);
        }

    }
}
