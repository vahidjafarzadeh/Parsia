using System;
using System.Linq;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.AccessGroup;

namespace Parsia.Core.Role
{
    public class RoleCopier : IBaseCopier<RoleDto, DataLayer.Model.Core.Role.Role>
    {
        public RoleDto GetDto(DataLayer.Model.Core.Role.Role entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto {EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username}
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto {EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username}
                : new UserDto();
            var dto = new RoleDto
            {
                EntityId = entity.EntityId,
                Active = entity.Active,
                Code = entity.Code,
                Deleted = entity.Deleted,
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                ExpireMinute = entity.ExpireMinute,
                RoleName = entity.RoleName
            };
            if (entity.ParentRoleAccessGroup != null)
            {
                var groupDto = entity.ParentRoleAccessGroup.Select(item => new AccessGroupDto
                    {EntityId = item.CurrentAccessGroup.EntityId, Name = item.CurrentAccessGroup.Name}).ToList();

                dto.AccessGroupDto = groupDto;
            }

            return dto;
        }

        public DataLayer.Model.Core.Role.Role GetEntity(RoleDto dto, BusinessParam bp, bool setCreate)
        {
            var role = new DataLayer.Model.Core.Role.Role
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.RoleName + " | " + dto.ExpireMinute + " | " + dto.Code + " | " + dto.EntityId,
                ExpireMinute = dto.ExpireMinute,
                RoleName = dto.RoleName
            };
            return SetMandatoryField(role, bp, setCreate);
        }

        public DataLayer.Model.Core.Role.Role SetMandatoryField(DataLayer.Model.Core.Role.Role role,
            BusinessParam bp, bool setCreate)
        {
            role.AccessKey = bp.UserInfo.AccessKey;
            role.UpdateBy = bp.UserInfo.UserId;
            role.Updated = DateTime.Now;
            if (!setCreate) return role;
            role.CreateBy = bp.UserInfo.UserId;
            role.Created = DateTime.Now;
            return role;
        }
    }
}