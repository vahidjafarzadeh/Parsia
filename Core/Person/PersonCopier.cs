using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.User;

namespace Parsia.Core.Person
{
    public class PersonCopier : IBaseCopier<PersonDto, DataLayer.Model.Core.Person.Person>
    {
        
        public PersonDto GetDto(DataLayer.Model.Core.Person.Person entity)
        {
            var userCopier = new UserCopier();
            var createUser = entity.CreateUserEntity != null
                ? userCopier.GetDto(entity.CreateUserEntity)
                : new UserDto();
            var updateUser = entity.CreateUserEntity != null
                ? userCopier.GetDto(entity.CreateUserEntity)
                : new UserDto();
            var personDto = new PersonDto
            {
                EntityId = entity.EntityId,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Active = entity.Active,
                Code = entity.Code,
            };
            return personDto;
        }

        public DataLayer.Model.Core.Person.Person GetEntity(PersonDto dto, BusinessParam bp, bool setCreate)
        {
            var person = new DataLayer.Model.Core.Person.Person
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.FirstName + " | " + dto.LastName + " | " + dto.FatherName + " | " + dto.NationalCode,
            };
            return SetMandatoryField(person, bp, setCreate);
        }

      

        public DataLayer.Model.Core.Person.Person SetMandatoryField(DataLayer.Model.Core.Person.Person person,
            BusinessParam bp, bool setCreate)
        {
            person.UpdateBy = bp.UserInfo.UserId;
            person.Updated = DateTime.Now;
            if (!setCreate) return person;
            person.CreateBy = bp.UserInfo.UserId;
            person.Created = DateTime.Now;
            return person;
        }
    }
}