using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.Organization;
using Parsia.Core.Role;

namespace Parsia.Core.BusinessAccess
{
    public class BusinessAccessCopier : IBaseCopier<BusinessAccessDto, DataLayer.Model.Core.BusinessAccess.BusinessAccess>
    {

        public BusinessAccessDto GetDto(DataLayer.Model.Core.BusinessAccess.BusinessAccess entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            var location = new BusinessAccessDto
            {
                EntityId = entity.EntityId,
                Active = entity.Active,
                Code = entity.Code,
                FullTitle = entity.FullTitle,
                Deleted = entity.Deleted,
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                UseCase = entity.UseCase,
                EntityIds = entity.EntityIds
            };
            if (entity.CurrentRole != null)
            {
                location.Role = new RoleDto() { EntityId = entity.CurrentRole.EntityId, RoleName = entity.CurrentRole.RoleName, FullTitle = entity.CurrentRole.FullTitle };
            }
            if (entity.CurrentOrganization != null)
            {
                location.Organization = new OrganizationDto() { EntityId = entity.CurrentOrganization.EntityId, Name = entity.CurrentOrganization.Name, FullTitle = entity.CurrentOrganization.FullTitle };
            }
            return location;
        }

        public DataLayer.Model.Core.BusinessAccess.BusinessAccess GetEntity(BusinessAccessDto dto, BusinessParam bp, bool setCreate)
        {
            var businessAccess = new DataLayer.Model.Core.BusinessAccess.BusinessAccess
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                UseCase = dto.UseCase,
                Role = dto.Role.EntityId,
                EntityIds = dto.EntityIds,
                Organization = dto.Organization.EntityId,
                FullTitle = dto.UseCase
            };
            return SetMandatoryField(businessAccess, bp, setCreate);
        }

        public DataLayer.Model.Core.BusinessAccess.BusinessAccess SetMandatoryField(DataLayer.Model.Core.BusinessAccess.BusinessAccess businessAccess,
            BusinessParam bp, bool setCreate)
        {
            businessAccess.AccessKey = bp.UserInfo.AccessKey;
            businessAccess.UpdateBy = bp.UserInfo.UserId;
            businessAccess.Updated = DateTime.Now;
            if (!setCreate) return businessAccess;
            businessAccess.CreateBy = bp.UserInfo.UserId;
            businessAccess.Created = DateTime.Now;
            return businessAccess;
        }
    }
}