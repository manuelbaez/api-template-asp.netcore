using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogic.DTO.General;
using BusinessLogic.Repositories.General;
using DataAccessUtil.Entity;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Services.General
{
    public class Service<TDto, TEntity>: IService<TDto> where TEntity :class, IBaseEntity where TDto : IDto
    {
        public IRepository<TEntity> _repository;

        public Service(IRepository<TEntity> repository)
        {

            _repository = repository;
        }

        public virtual object Add(TDto dto, bool saveChanges = false)
        {
            TEntity entity = Mapper.Map<TEntity>(dto);
            return _repository.Add(entity, saveChanges);
        }

        public virtual void AddRange(IEnumerable<TDto> entites)
        {
            var _entities = Mapper.Map<IEnumerable<TEntity>>(entites);
            _repository.AddRange(_entities);
            _repository.SaveChanges();
        }

        public virtual IQueryable<TDto> GetAll(IEnumerable<string> expandFields = null)
        {
            if (expandFields != null)
                return _repository.GetAll().ProjectTo<TDto>(null, expandFields.ToArray());
            else
                return _repository.GetAll().ProjectTo<TDto>();

        }
        public virtual IQueryable<TDto> GetAllActive(IEnumerable<string> expandFields = null)
        {
            if (expandFields != null)
                return _repository.GetAllActive().ProjectTo<TDto>(null, expandFields.ToArray());
            else
                return _repository.GetAllActive().ProjectTo<TDto>();
        }

        public virtual TDto Get(int id, IEnumerable<string> expandFields = null)
        {
            if (expandFields != null)
                return _repository.Get(id).ProjectTo<TDto>(null, expandFields.ToArray()).FirstOrDefault();
            else
                return _repository.Get(id).ProjectTo<TDto>().FirstOrDefault();
        }

        public virtual void Update(int id,TDto dto)
        {
            var entity = _repository.Get(id).First();
            var mappedEntity = Mapper.Map(dto, entity);
            _repository.Update(mappedEntity);
        }
        // Warning this actually replaces the entities in the database with te ones from the dto, 
        // keep in mind send the entire object otherwise it will erase any empty property from existence.
        public virtual void UpdateRange(IEnumerable<TDto> dto)
        {
            IEnumerable<TEntity> entity = Mapper.Map<IEnumerable<TEntity>>(dto);
            _repository.UpadateRange(entity);
            _repository.SaveChanges();
        }
        public virtual void Delete(int id)
        {
            _repository.Delete(id);
            _repository.SaveChanges();
        }

        public virtual void SaveChanges()
        {
            _repository.SaveChanges();
        }

        
    }
}
