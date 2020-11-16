using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.Organization;
using Parsia.Core.Role;

namespace Parsia.Core.EntityState
{
    public class EntityStateCopier : IBaseCopier<EntityStateDto, DataLayer.Model.Core.EntityState.EntityState>
    {

        public EntityStateDto GetDto(DataLayer.Model.Core.EntityState.EntityState entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            var entityState = new EntityStateDto
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
                TableEntityId = entity.TableEntityId,
                TableName = entity.TableName
            };
            if (entity.CurrentRole != null)
            {
                entityState.Role = new RoleDto(){EntityId = entity.CurrentRole.EntityId,RoleName = entity.CurrentRole.RoleName,FullTitle = entity.CurrentRole.FullTitle};
            }

            if (entity.CurrentOrganization != null)
            {
                entityState.Organization = new OrganizationDto() { EntityId = entity.CurrentOrganization.EntityId, Name = entity.CurrentOrganization.Name, FullTitle = entity.CurrentOrganization.FullTitle };
            }
            if (entity.CurrentUser != null)
            {
                entityState.User = new UserDto() { EntityId = entity.CurrentUser.EntityId, Username = entity.CurrentUser.Username, FullTitle = entity.CurrentUser.FullTitle };
            }
            return entityState;
        }

        public DataLayer.Model.Core.EntityState.EntityState GetEntity(EntityStateDto dto, BusinessParam bp, bool setCreate)
        {
            var entityState = new DataLayer.Model.Core.EntityState.EntityState
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.TableName,
                OrganizationId = dto.Organization.EntityId,
                RoleId = dto.Role.EntityId,
                UserId = dto.User.EntityId,
                TableName = dto.TableName,
                TableEntityId = dto.TableEntityId
            };
            return SetMandatoryField(entityState, bp, setCreate);
        }

        public DataLayer.Model.Core.EntityState.EntityState SetMandatoryField(DataLayer.Model.Core.EntityState.EntityState entityState,
            BusinessParam bp, bool setCreate)
        {
            entityState.AccessKey = bp.UserInfo.AccessKey;
            entityState.UpdateBy = bp.UserInfo.UserId;
            entityState.Updated = DateTime.Now;
            if (!setCreate) return entityState;
            entityState.CreateBy = bp.UserInfo.UserId;
            entityState.Created = DateTime.Now;
            return entityState;
        }
    }
}