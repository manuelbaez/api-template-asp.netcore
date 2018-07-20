using System;
using System.Collections.Generic;
using System.Linq;
using DataAccessUtil.Entity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Repositories.General
{
    public class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : class, IBaseEntity where TContext : DbContext
    {
        protected string User { get; }
        protected TContext _context;

        public Repository(TContext context)
        {
              
                _context = context;
       
        }

        public virtual object Add(TEntity entity, bool saveChanges = false)
        {
            entity.CreatedBy = User;
            entity.Active = true;
            _context.Set<TEntity>().Add(entity);
            if (saveChanges) _context.SaveChanges();
            return entity;
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _context.Set<TEntity>().Add(entity);
            }

        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().OrderBy(t => t.Id);
        }

        public virtual IQueryable<TEntity> GetAllActive()
        {
            return _context.Set<TEntity>().OrderBy(t=>t.Id).Where(t => t.Active);
        }

        public virtual IQueryable<TEntity> Get(object id)
        {
            var data = GetAll().Where(t => t.Id.ToString().Equals(id.ToString()));
            return data;
        }

        public virtual void UpadateRange(IEnumerable<TEntity> entity)
        {
            entity.ToList().ForEach(x =>
            {
                if ((int)x.Id <= 0)
                {
                    Add(x);

                }
                else
                {
                    Update(x);
                }
            });

        }

        public virtual void Update(TEntity entity)
        {
            entity.ModifiedBy = User;
            entity.DateModified = DateTime.Now;
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(object id)
        {
            var entity = _context.Set<TEntity>().Find(id);
            entity.Active = false;
            entity.ModifiedBy = User;
            entity.DateModified = DateTime.Now;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

    }
}
