using Core.DTO;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IPersonService
    {
        Task<List<PersonDto>?> GetPersonList(Person Person);
        Task<bool> DeletePerson(Guid? personId);
        Task<bool> UpdatePerson(PersonDto personDto);
        Task<bool> AddPerson(PersonDto personDto);

    }
}
