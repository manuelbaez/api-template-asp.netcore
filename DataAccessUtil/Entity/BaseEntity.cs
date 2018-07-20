using DataAccessUtil.Context;
using DataAccessUtil.Entity.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DataAccessUtil.Entity
{
    public abstract class BaseEntity<TPrimarykey, TEntity, TContext>
        : Entity<TEntity, TContext>, IBaseEntity where TEntity : class, IBaseEntity where TContext : Context.SelfGeneratedDbContext
    {
        public TPrimarykey Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }
        object IBaseEntity.Id => Id;

        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(m => m.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(m => m.Active).HasDefaultValueSql("1");
            builder.Property(m => m.CreatedBy).IsRequired().HasDefaultValue("NotRegistered").HasMaxLength(50);
            builder.Property(m => m.ModifiedBy).HasMaxLength(50);
            base.Configure(builder);
        }
    }

    public abstract class BaseEntity<TEntity, TContext>
       : BaseEntity<int,TEntity, TContext> where TEntity : class, IBaseEntity where TContext : Context.SelfGeneratedDbContext
    {
    }
}
