using DataAccessUtil.Entity;
using DataAccessUtil.Entity.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DataAccessUtil.Context
{

    public abstract class SelfGeneratedDbContext : DbContext
    {
        public SelfGeneratedDbContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var contextType = GetType();
            var assembliesTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes());

            var entities = assembliesTypes.Where(t =>
            {
                var baseType = t.BaseType;
                return
                  baseType != null &&
                  baseType.IsGenericType &&
                  !t.IsAbstract &&
                  !t.IsInterface && (
                  baseType.GetGenericTypeDefinition().Equals(typeof(BaseEntity<,,>)) ||
                  baseType.GetGenericTypeDefinition().Equals(typeof(BaseEntity<,>)) ||
                  baseType.GetGenericTypeDefinition().Equals(typeof(Entity<,>))
                  ) &&
                  t.BaseType.GetGenericArguments().Any(x => x.Equals(contextType));
            });

            var applyConfigurationMethod = typeof(ModelBuilder).GetMethods()
                .First(t => t.Name.Equals(nameof(modelBuilder.ApplyConfiguration)) &&
                t.GetParameters()[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IEntityTypeConfiguration<>)));
            foreach (var entity in entities)
            {
                applyConfigurationMethod.MakeGenericMethod(entity)
                    .Invoke(modelBuilder, new object[] { Activator.CreateInstance(entity) });
            }

            base.OnModelCreating(modelBuilder);
        }
    }

}
