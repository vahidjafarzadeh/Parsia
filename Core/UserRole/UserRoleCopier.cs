using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.Organization;
using Parsia.Core.Role;

namespace Parsia.Core.UserRole
{
    public class UserRoleCopier : IBaseCopier<UserRoleDto, DataLayer.Model.Core.UserRole.UserRole>
    {
        public UserRoleDto GetDto(DataLayer.Model.Core.UserRole.UserRole entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto {EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username}
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto {EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username}
                : new UserDto();
            var dto = new UserRoleDto
            {
                EntityId = entity.EntityId,
                Active = entity.Active,
                Code = entity.Code,
                Deleted = entity.Deleted,
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated)
            };
            if (entity.CurrentOrganization != null)
            {
                dto.Organization = new OrganizationDto
                {
                    EntityId = entity.CurrentOrganization.EntityId, Name = entity.CurrentOrganization.Name,
                    FullTitle = entity.CurrentOrganization.FullTitle, AccessKey = entity.CurrentOrganization.AccessKey
                };
                dto.OrgAccess = dto.Organization.AccessKey;
            }

            if (entity.CurrentRole != null)
                dto.Role = new RoleDto
                {
                    EntityId = entity.CurrentRole.EntityId, RoleName = entity.CurrentRole.RoleName,
                    FullTitle = entity.CurrentRole.FullTitle
                };
            if (entity.CurrentUsers != null)
                dto.User = new UserDto
                {
                    EntityId = entity.CurrentUsers.EntityId, FirstName = entity.CurrentUsers.FirstName,
                    FullTitle = entity.CurrentUsers.FullTitle, LastName = entity.CurrentUsers.LastName,
                    Username = entity.CurrentUsers.Username
                };
            return dto;
        }

        public DataLayer.Model.Core.UserRole.UserRole GetEntity(UserRoleDto dto, BusinessParam bp, bool setCreate)
        {
            var uerRole = new DataLayer.Model.Core.UserRole.UserRole
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Role?.EntityId + " | " + dto.User?.EntityId + " | " + dto.Organization?.EntityId +
                            " | " + dto.OrgAccess,
                OrgAccess = dto.OrgAccess,
                OrganizationId = dto.Organization.EntityId,
                UserId = dto.User.EntityId,
                RoleId = dto.Role.EntityId
            };
            return SetMandatoryField(uerRole, bp, setCreate);
        }

        public DataLayer.Model.Core.UserRole.UserRole SetMandatoryField(DataLayer.Model.Core.UserRole.UserRole userRole,
            BusinessParam bp, bool setCreate)
        {
            userRole.AccessKey = bp.UserInfo.AccessKey;
            userRole.UpdateBy = bp.UserInfo.UserId;
            userRole.Updated = DateTime.Now;
            if (!setCreate) return userRole;
            userRole.CreateBy = bp.UserInfo.UserId;
            userRole.Created = DateTime.Now;
            return userRole;
        }
    }
}