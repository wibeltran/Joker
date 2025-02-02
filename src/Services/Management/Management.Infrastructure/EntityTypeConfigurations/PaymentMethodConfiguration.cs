﻿using Management.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Management.Infrastructure.EntityTypeConfigurations;

public partial class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> entity)
    {
        entity.Property(e => e.Id).HasDefaultValue(Guid.NewGuid());
        entity.Property(e => e.Code).HasMaxLength(10);
        entity.Property(e => e.ImageUrl).HasMaxLength(250);
        entity.Property(e => e.Name).HasMaxLength(250);
    }
}