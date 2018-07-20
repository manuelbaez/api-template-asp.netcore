using System;

namespace DataAccessUtil.Entity
{
    public interface IBaseEntity
    {
        object Id { get; }
        DateTime DateCreated { get; set; }
        string CreatedBy { get; set; }
        DateTime? DateModified { get; set; }
        string ModifiedBy { get; set; }
        bool Active { get; set; }
    }
}
