using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.ComboVal;
using Parsia.Core.File;

namespace Parsia.Core.Person
{
    public class PersonCopier : IBaseCopier<PersonDto, DataLayer.Model.Core.Person.Person>
    {
        
        public PersonDto GetDto(DataLayer.Model.Core.Person.Person entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId,Username = entity.CreateUserEntity.Username}
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            return new PersonDto
            {
                EntityId = entity.EntityId,
                Active = entity.Active,
                Code = entity.Code,
                Deleted = entity.Deleted,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                NationalCode = entity.NationalCode,
                BirthPlace = entity.BirthPlace,
                BirthDate = Util.GetTimeStamp(entity.BirthDate),
                PersianCode = entity.PersianCode,
                FatherName = entity.FatherName,
                MotherName = entity.MotherName,
                ExportationPlace = entity.ExportationPlace,
                CertificateSeries = entity.CertificateSeries,
                IdentitySerialNumber = entity.IdentitySerialNumber,
                Mobile = entity.Mobile,
                EmergencyPhone = entity.EmergencyPhone,
                Email = entity.Email,
                LeftHanded = entity.LeftHanded,
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                Sex = (entity.CurrentSex != null? new ComboValDto() {EntityId = entity.CurrentSex.EntityId,Name = entity.CurrentSex.Name}: null),
                Nationality = (entity.CurrentNationality != null ? new ComboValDto() {EntityId = entity.CurrentNationality.EntityId, Name = entity.CurrentNationality.Name } : null),
                BloodType = (entity.CurrentBloodType != null? new ComboValDto() {EntityId = entity.CurrentBloodType.EntityId, Name = entity.CurrentBloodType.Name } : null),
                LifeStatus = (entity.CurrentLifeStatus != null? new ComboValDto() {EntityId = entity.CurrentLifeStatus.EntityId, Name = entity.CurrentLifeStatus.Name } : null),
                Citizenship = (entity.CurrentCitizenship != null? new ComboValDto() {EntityId = entity.CurrentCitizenship.EntityId, Name = entity.CurrentCitizenship.Name } : null),
                Religion = (entity.CurrentReligion != null? new ComboValDto() {EntityId = entity.CurrentReligion.EntityId, Name = entity.CurrentReligion.Name } : null),
                SubReligion = (entity.CurrentSubReligion != null? new ComboValDto() { EntityId = entity.CurrentSubReligion.EntityId, Name = entity.CurrentSubReligion.Name } : null),
                MaritalStatus = (entity.CurrentMaritalStatus != null? new ComboValDto() { EntityId = entity.CurrentMaritalStatus.EntityId, Name = entity.CurrentMaritalStatus.Name } : null),
                MilitaryServiceStatus = (entity.CurrentMilitaryServiceStatus != null? new ComboValDto() { EntityId = entity.CurrentMilitaryServiceStatus.EntityId, Name = entity.CurrentMilitaryServiceStatus.Name } : null),
                HousingSituation = (entity.CurrentHousingSituation != null? new ComboValDto() { EntityId = entity.CurrentHousingSituation.EntityId, Name = entity.CurrentHousingSituation.Name } : null),
                HealthStatus =(entity.CurrentHealthStatus != null? new ComboValDto() { EntityId = entity.CurrentHealthStatus.EntityId, Name = entity.CurrentHealthStatus.Name } : null),
                DisabilityType = (entity.CurrentDisabilityType != null? new ComboValDto() { EntityId = entity.CurrentDisabilityType.EntityId, Name = entity.CurrentDisabilityType.Name } : null),
                Description = entity.Description,
                File =(entity.CurrentFile != null? new FileDto(){ EntityId = entity.CurrentFile.EntityId,Description = entity.CurrentFile.Description,Name = entity.CurrentFile.Name ,Thumbnail = entity.CurrentFile.Thumbnail, } : null)
            };
            
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
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                NationalCode = dto.NationalCode,
                BirthPlace = dto.BirthPlace,
                BirthDate = Util.GetDateTime(dto.BirthDate),
                PersianCode = dto.PersianCode,
                FatherName = dto.FatherName,
                MotherName = dto.MotherName,
                ExportationPlace = dto.ExportationPlace,
                CertificateSeries = dto.CertificateSeries,
                IdentitySerialNumber = dto.IdentitySerialNumber,
                Mobile = dto.Mobile,
                EmergencyPhone = dto.EmergencyPhone,
                Email = dto.Email,
                LeftHanded = dto.LeftHanded,
                Sex = dto.Sex?.EntityId,
                Nationality = dto.Nationality?.EntityId,
                BloodType = dto.BloodType?.EntityId,
                LifeStatus = dto.LifeStatus?.EntityId,
                Citizenship = dto.Citizenship?.EntityId,
                Religion = dto.Religion?.EntityId,
                SubReligion = dto.SubReligion?.EntityId,
                MaritalStatus = dto.MaritalStatus?.EntityId,
                MilitaryServiceStatus = dto.MilitaryServiceStatus?.EntityId,
                HousingSituation = dto.HousingSituation?.EntityId,
                HealthStatus = dto.HealthStatus?.EntityId,
                Description = dto.Description,
                File = dto.File?.EntityId
            };
            if (string.IsNullOrEmpty(person.Code))
            {
                person.Code = "-";
            }
            return SetMandatoryField(person, bp, setCreate);
        }

        public DataLayer.Model.Core.Person.Person SetMandatoryField(DataLayer.Model.Core.Person.Person person,
            BusinessParam bp, bool setCreate)
        {
            person.AccessKey = bp.UserInfo.AccessKey;
            person.UpdateBy = bp.UserInfo.UserId;
            person.Updated = DateTime.Now;
            if (!setCreate) return person;
            person.CreateBy = bp.UserInfo.UserId;
            person.Created = DateTime.Now;
            return person;
        }
    }
}