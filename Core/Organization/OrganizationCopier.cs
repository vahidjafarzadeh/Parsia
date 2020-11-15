using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.ComboVal;
using Parsia.Core.File;
using Parsia.Core.Location;

namespace Parsia.Core.Organization
{
    public class OrganizationCopier : IBaseCopier<OrganizationDto, DataLayer.Model.Core.Organization.Organization>
    {
        public OrganizationDto GetDto(DataLayer.Model.Core.Organization.Organization entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            var organizationDto = new OrganizationDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Active = entity.Active,
                Code = entity.Code,
                Longitude = entity.Latitude,
                Mobile = entity.Mobile,
                NumberOfFloors = entity.NumberOfFloors,
                AboutUs = entity.AboutUs,
                Address = entity.Address,
                Area = entity.Area,
                Deleted = entity.Deleted,
                Description = entity.Description,
                Email = entity.Email,
                EstablishingYear = Util.GetTimeStamp(entity.EstablishingYear),
                Fax = entity.Fax,
                Latitude = entity.Latitude,
                NumberOfRooms = entity.NumberOfRooms,
                Order = entity.Order,
                ParentId = entity.ParentId,
                YearOfConstruction = Util.GetTimeStamp(entity.YearOfConstruction),
                TelPhone = entity.TelPhone,
                SmsNumber = entity.SmsNumber,
            };
            if (entity.CurrentLogo != null)
            {
                organizationDto.Logo = new FileDto()
                {
                    EntityId = entity.CurrentLogo.EntityId,
                    Name = entity.CurrentLogo.Name,
                    Thumbnail = entity.CurrentLogo.Thumbnail,
                    Description = entity.CurrentLogo.Description
                };
            }

            if (entity.CurrentCity != null)
            {
                organizationDto.City = new LocationDto()
                {
                    EntityId = entity.CurrentCity.EntityId,
                    Name = entity.CurrentCity.Name,
                    FullTitle = entity.CurrentCity.FullTitle
                };
            }

            if (entity.CurrentOrganization != null)
            {
                organizationDto.Parent = new OrganizationDto()
                {
                    EntityId = entity.CurrentOrganization.EntityId,
                    Name = entity.CurrentOrganization.Name,
                    FullTitle = entity.CurrentOrganization.FullTitle
                };
            }

            if (entity.CurrentOrganizationGrade != null)
            {
                organizationDto.OrganizationGrade = new ComboValDto()
                {
                    EntityId = entity.CurrentOrganizationGrade.EntityId,
                    Name = entity.CurrentOrganizationGrade.Name,
                    FullTitle = entity.CurrentOrganizationGrade.FullTitle
                };
            }

            if (entity.CurrentOrganizationOwnershipType != null)
            {
                organizationDto.OrganizationOwnershipType = new ComboValDto()
                {
                    EntityId = entity.CurrentOrganizationOwnershipType.EntityId,
                    Name = entity.CurrentOrganizationOwnershipType.Name,
                    FullTitle = entity.CurrentOrganizationOwnershipType.FullTitle
                };
            }

            if (entity.CurrentOrganizationRoadType != null)
            {
                organizationDto.OrganizationRoadType = new ComboValDto()
                {
                    EntityId = entity.CurrentOrganizationRoadType.EntityId,
                    Name = entity.CurrentOrganizationRoadType.Name,
                    FullTitle = entity.CurrentOrganizationRoadType.FullTitle
                };
            }

            if (entity.CurrentOrganizationStatus != null)
            {
                organizationDto.OrganizationStatus = new ComboValDto()
                {
                    EntityId = entity.CurrentOrganizationStatus.EntityId,
                    Name = entity.CurrentOrganizationStatus.Name,
                    FullTitle = entity.CurrentOrganizationStatus.FullTitle
                };
            }

            if (entity.CurrentOrganizationType != null)
            {
                organizationDto.OrganizationType = new ComboValDto()
                {
                    EntityId = entity.CurrentOrganizationType.EntityId,
                    Name = entity.CurrentOrganizationType.Name,
                    FullTitle = entity.CurrentOrganizationType.FullTitle
                };
            }

            if (entity.CurrentProvince != null)
            {
                organizationDto.Province = new LocationDto()
                {
                    EntityId = entity.CurrentProvince.EntityId,
                    Name = entity.CurrentProvince.Name,
                    FullTitle = entity.CurrentProvince.FullTitle
                };
            }

            return organizationDto;
        }

        public DataLayer.Model.Core.Organization.Organization GetEntity(OrganizationDto dto, BusinessParam bp, bool setCreate)
        {
            var organization = new DataLayer.Model.Core.Organization.Organization
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Name + " | " + dto.Code + " | " + dto.EntityId,
                Name = dto.Name,
                ParentId = dto.Parent?.EntityId,
                AboutUs = dto.AboutUs,
                AccessKey = OrganizationFacade.GetInstance().SetAccessKey(dto,dto.Parent?.EntityId,bp),
                Address = dto.Address,
                Area = dto.Area,
                City = dto.City?.EntityId,
                Description = dto.Description,
                YearOfConstruction = Util.GetDateTime(dto.YearOfConstruction),
                TelPhone = dto.TelPhone,
                SmsNumber = dto.SmsNumber,
                Province = dto.Province?.EntityId,
                OrganizationType = dto.OrganizationType?.EntityId,
                OrganizationStatus = dto.OrganizationStatus?.EntityId,
                OrganizationRoadType = dto.OrganizationRoadType?.EntityId,
                OrganizationOwnershipType = dto.OrganizationOwnershipType?.EntityId,
                OrganizationGrade = dto.OrganizationGrade?.EntityId,
                Order = dto.Order,
                NumberOfRooms = dto.NumberOfRooms,
                NumberOfFloors = dto.NumberOfFloors,
                Mobile = dto.Mobile,
                Longitude = dto.Latitude,
                Logo = dto.Logo?.EntityId,
                Latitude = dto.Latitude,
                Fax = dto.Fax,
                EstablishingYear = Util.GetDateTime(dto.EstablishingYear),
                Email = dto.Email
            };
            return SetMandatoryField(organization, bp, setCreate);
        }

        public DataLayer.Model.Core.Organization.Organization SetMandatoryField(DataLayer.Model.Core.Organization.Organization organization,
            BusinessParam bp, bool setCreate)
        {
            organization.UpdateBy = bp.UserInfo.UserId;
            organization.Updated = DateTime.Now;
            if (!setCreate) return organization;
            organization.CreateBy = bp.UserInfo.UserId;
            organization.Created = DateTime.Now;
            return organization;
        }
    }
}