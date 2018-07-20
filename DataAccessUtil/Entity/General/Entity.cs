using DataAccessUtil.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessUtil.Entity.General
{
    public abstract class Entity<TEntity, TContext> : IEntityTypeConfiguration<TEntity> where TEntity : class
        where TContext : SelfGeneratedDbContext
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
        }
    }
}
