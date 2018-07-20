using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DTO.General;

namespace BusinessLogic.Services.General
{
    public interface IService<TDto> where TDto : IDto
    {
        object Add(TDto dto, bool saveChanges = false);
        void AddRange(IEnumerable<TDto> entities);
        IQueryable<TDto> GetAll(IEnumerable<string> expandFields = null);
        IQueryable<TDto> GetAllActive(IEnumerable<string> expandFields = null);
        TDto Get(int id, IEnumerable<string> expandFields = null);
        void Update(int id, TDto dto);
        void UpdateRange(IEnumerable<TDto> dto);
        void Delete(int id);
        void SaveChanges();
    }
}
