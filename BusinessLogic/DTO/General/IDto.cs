using System;

namespace BusinessLogic.DTO.General
{
    public interface IDto
    {
        object Id { get; }
        bool Active { get; set; }
    }
}
