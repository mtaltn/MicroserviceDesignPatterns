using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MassTransit;

namespace SagaStateMachineWorkerService.Models
{
    public abstract class SagaClassMapBase<TEntity> : SagaClassMap<TEntity> where TEntity : class, SagaStateMachineInstance
    {
        protected override void Configure(EntityTypeBuilder<TEntity> entity, ModelBuilder model)
        {
            if (typeof(TEntity).GetProperty("BuyerId") is not null)
            {
                entity.Property("BuyerId").HasMaxLength(256);
            }
        }
    }
}
