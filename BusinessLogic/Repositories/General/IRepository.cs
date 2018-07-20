using DataAccessUtil.Entity;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Repositories.General
{
    public interface IRepository<TEntity> where TEntity : class, IBaseEntity
    {
        object Add(TEntity entity, bool saveChanges = false);
        void AddRange(IEnumerable<TEntity> entities);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllActive();
        IQueryable<TEntity> Get(object id);
        void Update(TEntity entity);
        void UpadateRange(IEnumerable<TEntity> entity);
        void Delete(object id);
        void SaveChanges();
    }
}
