using System;

namespace BusinessLogic.DTO.General
{
    public class Dto<TPrimaryKey> : IDto
    {
        public TPrimaryKey Id { get; set; }
        public bool Active { get; set; }
        object IDto.Id => Id;
    }
}
