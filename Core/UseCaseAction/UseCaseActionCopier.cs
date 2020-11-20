using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.Action;
using Parsia.Core.UseCase;

namespace Parsia.Core.UseCaseAction
{
    public class UseCaseActionCopier : IBaseCopier<UseCaseActionDto, DataLayer.Model.Core.UseCaseAction.UseCaseAction>
    {
        public UseCaseActionDto GetDto(DataLayer.Model.Core.UseCaseAction.UseCaseAction entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto {EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username}
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto {EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username}
                : new UserDto();
            var useCaseAction = new UseCaseActionDto
            {
                EntityId = entity.EntityId,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Active = entity.Active,
                Code = entity.Code
            };
            if (entity.CurrentAction != null)
                useCaseAction.Action = new ActionDto
                {
                    EntityId = entity.CurrentAction.EntityId,
                    ActionEnName = entity.CurrentAction.ActionEnName,
                    ActionName = entity.CurrentAction.ActionName
                };
            if (entity.CurrentUseCase != null)
                useCaseAction.UseCase = new UseCaseDto
                {
                    EntityId = entity.CurrentUseCase.EntityId,
                    UseCaseName = entity.CurrentUseCase.UseCaseName,
                    Clazz = entity.CurrentUseCase.Clazz
                };
            return useCaseAction;
        }

        public DataLayer.Model.Core.UseCaseAction.UseCaseAction GetEntity(UseCaseActionDto dto, BusinessParam bp,
            bool setCreate)
        {
            var useCaseAction = new DataLayer.Model.Core.UseCaseAction.UseCaseAction
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Action?.EntityId + " | " + dto.UseCase?.EntityId + " | " + dto.Code + " | " +
                            dto.EntityId,
                UseCase = dto.UseCase?.EntityId,
                Action = dto.Action?.EntityId
            };
            return SetMandatoryField(useCaseAction, bp, setCreate);
        }

        public DataLayer.Model.Core.UseCaseAction.UseCaseAction SetMandatoryField(
            DataLayer.Model.Core.UseCaseAction.UseCaseAction useCaseAction,
            BusinessParam bp, bool setCreate)
        {
            useCaseAction.AccessKey = bp.UserInfo.AccessKey;
            useCaseAction.UpdateBy = bp.UserInfo.UserId;
            useCaseAction.Updated = DateTime.Now;
            if (!setCreate) return useCaseAction;
            useCaseAction.CreateBy = bp.UserInfo.UserId;
            useCaseAction.Created = DateTime.Now;
            return useCaseAction;
        }
    }
}