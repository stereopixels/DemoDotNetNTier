﻿using System;
using System.Collections.Generic;
using DemoApp.Services.DTO;
using DemoApp.Data.Entities;
using AutoMapper;
using DemoApp.Data;
using System.Linq;
using DemoApp.Services.CustomExceptions;

namespace DemoApp.Services
{
    public class PersonnelService : IPersonnelService
    {
        private readonly IUnitOfWork _uow;

        public PersonnelService(IUnitOfWork uow)
        {
            _uow = uow;
            AutoMapperConfiguration.Configure();
        }

        public void AddPerson(PersonDto person)
        {
            Person p = Mapper.Map<Person>(person);

            if (_uow.PersonRepository.GetAll().FirstOrDefault(x => x.EmailAddress
                .Equals(p.EmailAddress, StringComparison.OrdinalIgnoreCase)) == null)
            {
                _uow.PersonRepository.Add(p);
                _uow.SaveChanges();
            } else
            {
                throw new PersonExistsException("A person with this email address already exists");
            }
        }

        /// <summary>
        /// Gets a list of all people in the system, ordered by last and then first name.
        /// </summary>
        /// <returns></returns>
        public IList<PersonDto> GetAll()
        {
            var elem = _uow.PersonRepository.GetAll();
            return Mapper.Map<List<PersonDto>>(_uow.PersonRepository.GetAll()
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList());
        }

        /// <summary>
        /// Removes a person.
        /// </summary>
        /// <param name="person"></param>
        public void RemovePerson(PersonDto person)
        {
            var p = _uow.PersonRepository.FindById(person.PersonDtoId);
            if (p != null)
            {
                _uow.PersonRepository.Remove(p);
                _uow.SaveChanges();
            }
            else
            {
                throw new PersonNotExistsException("The person to remove could not be found");
            }
        }

        /// <summary>
        /// Updates a person.
        /// </summary>
        /// <param name="person"></param>
        public void UpdatePerson(PersonDto person)
        {
            var p = _uow.PersonRepository.FindById(person.PersonDtoId);
            if (p != null)
            {
                var personToUpdate = Mapper.Map<Person>(person);
                _uow.PersonRepository.Update(personToUpdate);
                _uow.SaveChanges();
            }
            else
            {
                throw new PersonNotExistsException("The person to update could not be found");
            }
        }
    }
}
