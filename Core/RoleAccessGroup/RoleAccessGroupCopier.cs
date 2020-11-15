using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.AccessGroup;
using Parsia.Core.Role;

namespace Parsia.Core.RoleAccessGroup
{
    public class RoleAccessGroupCopier : IBaseCopier<RoleAccessGroupDto, DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup>
    {
        public RoleAccessGroupDto GetDto(DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            var roleAccessGroupDto = new RoleAccessGroupDto()
            {
                EntityId = entity.EntityId,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Active = entity.Active,
                Code = entity.Code,
            };
            if (entity.CurrentAccessGroup != null)
                roleAccessGroupDto.AccessGroup = new AccessGroupDto()
                {
                    EntityId = entity.CurrentAccessGroup.EntityId,
                    Name = entity.CurrentAccessGroup.Name
                };
            if (entity.CurrentRole != null)
                roleAccessGroupDto.Role = new RoleDto()
                {
                    EntityId = entity.CurrentRole.EntityId
                };
            return roleAccessGroupDto;
        }

        public DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup GetEntity(RoleAccessGroupDto dto, BusinessParam bp, bool setCreate)
        {
            var roleAccessGroup = new DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup()
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.AccessGroup?.EntityId + " | " + dto.Role?.EntityId + " | " + dto.Code + " | " + dto.EntityId,
                AccessGroup = dto.AccessGroup.EntityId,
                Role = dto.Role.EntityId
            };
            return SetMandatoryField(roleAccessGroup, bp, setCreate);
        }
        public DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup SetMandatoryField(DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup roleAccessGroup,
            BusinessParam bp, bool setCreate)
        {
            roleAccessGroup.AccessKey = bp.UserInfo.AccessKey;
            roleAccessGroup.UpdateBy = bp.UserInfo.UserId;
            roleAccessGroup.Updated = DateTime.Now;
            if (!setCreate) return roleAccessGroup;
            roleAccessGroup.CreateBy = bp.UserInfo.UserId;
            roleAccessGroup.Created = DateTime.Now;
            return roleAccessGroup;
        }
    }
}

