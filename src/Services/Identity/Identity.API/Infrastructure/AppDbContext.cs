﻿using Core.Autofac;
using Core.EntityFramework.Context;
using Identity.Domain.Entities.UserAggregate;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Identity.Infrastructure
{
    public class AppDbContext : BaseDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }
        public override void Dispose()
        {
            base.Dispose();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}
