﻿using EmployeeMgt.Domain.Entities;
using EmployeeMgt.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeMgt.Infrastructure.Configurations
{

    internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable(nameof(Employee));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(48);
            builder.Property(x => x.Sex).HasDefaultValue(Sex.Male);
        }
    }
}
