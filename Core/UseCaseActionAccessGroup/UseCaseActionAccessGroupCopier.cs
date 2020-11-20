using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.AccessGroup;
using Parsia.Core.UseCaseAction;

namespace Parsia.Core.UseCaseActionAccessGroup
{
    public class UseCaseActionAccessGroupCopier : IBaseCopier<UseCaseActionAccessGroupDto,
        DataLayer.Model.Core.UseCaseActionAccessGroup.UseCaseActionAccessGroup>
    {
        public UseCaseActionAccessGroupDto GetDto(
            DataLayer.Model.Core.UseCaseActionAccessGroup.UseCaseActionAccessGroup entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto {EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username}
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto {EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username}
                : new UserDto();
            var useCaseActionAccessGroup = new UseCaseActionAccessGroupDto
            {
                EntityId = entity.EntityId,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Active = entity.Active,
                Code = entity.Code
            };
            if (entity.CurrentAccessGroup != null)
                useCaseActionAccessGroup.AccessGroup = new AccessGroupDto
                {
                    EntityId = entity.CurrentAccessGroup.EntityId,
                    Name = entity.CurrentAccessGroup.Name
                };
            if (entity.CurrentUseCaseAction != null)
                useCaseActionAccessGroup.UseCaseAction = new UseCaseActionDto
                {
                    EntityId = entity.CurrentUseCaseAction.EntityId
                };
            return useCaseActionAccessGroup;
        }

        public DataLayer.Model.Core.UseCaseActionAccessGroup.UseCaseActionAccessGroup GetEntity(
            UseCaseActionAccessGroupDto dto, BusinessParam bp, bool setCreate)
        {
            var useCaseActionAccessGroup = new DataLayer.Model.Core.UseCaseActionAccessGroup.UseCaseActionAccessGroup
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.AccessGroup?.EntityId + " | " + dto.UseCaseAction?.EntityId + " | " + dto.Code + " | " +
                            dto.EntityId,
                AccessGroup = dto.AccessGroup.EntityId,
                UseCaseAction = UseCaseActionFacade.GetInstance().GetUseCaseAction(dto.UseCaseAction)
            };
            return SetMandatoryField(useCaseActionAccessGroup, bp, setCreate);
        }

        public DataLayer.Model.Core.UseCaseActionAccessGroup.UseCaseActionAccessGroup SetMandatoryField(
            DataLayer.Model.Core.UseCaseActionAccessGroup.UseCaseActionAccessGroup useCaseActionAccessGroup,
            BusinessParam bp, bool setCreate)
        {
            useCaseActionAccessGroup.AccessKey = bp.UserInfo.AccessKey;
            useCaseActionAccessGroup.UpdateBy = bp.UserInfo.UserId;
            useCaseActionAccessGroup.Updated = DateTime.Now;
            if (!setCreate) return useCaseActionAccessGroup;
            useCaseActionAccessGroup.CreateBy = bp.UserInfo.UserId;
            useCaseActionAccessGroup.Created = DateTime.Now;
            return useCaseActionAccessGroup;
        }
    }
}