using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace SagaStateMachineWorkerService.Models;

public class OrderStateMap : SagaClassMap<OrderStateInstance>
{
    protected override void Configure(EntityTypeBuilder<OrderStateInstance> entity, ModelBuilder model)
    {
        entity.Property(x => x.BuyerId).HasMaxLength(256);
    }
}
