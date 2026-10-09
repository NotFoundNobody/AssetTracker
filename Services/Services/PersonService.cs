using AssetTracker.Data;
using Core.DTO;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class PersonService(AppDbContext context)  : IPersonService
    {
        public async Task<List<PersonDto>?> GetPersonList(Person Person)
        {
            if (Person == null)
                return null;

            return await context.Persons
                .Where(x => x.PersonID != Person.PersonID&&x.IsDeleted==false)
                .Select(x => new PersonDto
                {
                    PersonID=x.PersonID,
                   FirstName= x.FirstName,
                    LastName = x.LastName,
                    NationaleCode = x.NationaleCode,
                    CellPhone = x.CellPhone
                })
                .ToListAsync();
        }
        public async Task<bool> DeletePerson(Guid? personId)
        {
            var person = await context.Persons
                .FirstOrDefaultAsync(x => x.PersonID == personId && !x.IsDeleted);

            if (person == null)
                return false;

            person.IsDeleted = true;

            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UpdatePerson(PersonDto personDto)
        {
            var person = await context.Persons
                .FirstOrDefaultAsync(x =>
                    x.PersonID == personDto.PersonID &&
                    !x.IsDeleted);

            if (person == null)
                return false;

            if (person.FirstName != personDto.FirstName)
                person.FirstName = personDto.FirstName;

            if (person.LastName != personDto.LastName)
                person.LastName = personDto.LastName;

            if (person.NationaleCode != personDto.NationaleCode)
                person.NationaleCode = personDto.NationaleCode;

            if (person.CellPhone != personDto.CellPhone)
                person.CellPhone = personDto.CellPhone;

            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> AddPerson(PersonDto personDto)
        {
            if (personDto == null)
                return false;

            var person = new Person
            {
                FirstName = personDto.FirstName,
                LastName = personDto.LastName,
                NationaleCode = personDto.NationaleCode,
                CellPhone = personDto.CellPhone
            };

            await context.Persons.AddAsync(person);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
