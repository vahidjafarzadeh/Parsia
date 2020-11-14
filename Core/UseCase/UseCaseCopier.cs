using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.Action;
using Parsia.Core.UseCaseAction;

namespace Parsia.Core.UseCase
{
    public class UseCaseCopier : IBaseCopier<UseCaseDto, DataLayer.Model.Core.UseCase.UseCase>
    {
        public UseCaseDto GetDto(DataLayer.Model.Core.UseCase.UseCase entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            var useCase = new UseCaseDto()
            {
                EntityId = entity.EntityId,
                Clazz = entity.Clazz,
                VirtualNode = entity.VirtualNode,
                TableName = entity.TableName,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Active = entity.Active,
                Code = entity.Code,
                UseCaseName = entity.UseCaseName,
            };
            if (entity.CurrentUseCase != null)
                useCase.Parent = new UseCaseDto()
                {
                    EntityId = entity.CurrentUseCase.EntityId,
                    UseCaseName = entity.CurrentUseCase.UseCaseName
                };
            if (entity.ParentUseCaseUseCaseAction != null)
            {
                var dataDto = entity.ParentUseCaseUseCaseAction.Select(item => new UseCaseActionDto() { UseCase = new UseCaseDto() { EntityId = Convert.ToInt64(item.UseCase) }, Action = new ActionDto() { ActionName = (item.CurrentAction != null ? item.CurrentAction.ActionName : ""), EntityId = Convert.ToInt64(item.Action) } }).ToList();

                useCase.UseCaseActions = dataDto;
            }
            return useCase;
        }
        public List<UseCaseDto> GetDto(List<DataLayer.Model.Core.UseCase.UseCase> lastData)
        {
            var lstDto = new List<UseCaseDto>();
            foreach (var entity in lastData)
            {
                var useCase = new UseCaseDto()
                {
                    EntityId = entity.EntityId,
                    Clazz = entity.Clazz,
                    VirtualNode = entity.VirtualNode,
                    TableName = entity.TableName,
                    Created = Util.GetTimeStamp(entity.Created),
                    Updated = Util.GetTimeStamp(entity.Updated),
                    CreatedBy = null,
                    UpdatedBy = null,
                    FullTitle = entity.UseCaseName,
                    Active = entity.Active,
                    Code = entity.Code,
                    UseCaseName = entity.UseCaseName,
                };
                if (entity.CurrentUseCase != null)
                    useCase.Parent = new UseCaseDto()
                    {
                        EntityId = entity.CurrentUseCase.EntityId,
                        UseCaseName = entity.CurrentUseCase.UseCaseName
                    };
                if (entity.ParentUseCaseUseCaseAction != null)
                {
                    var dataDto = entity.ParentUseCaseUseCaseAction.Select(item => new UseCaseActionDto() { UseCase = new UseCaseDto() { EntityId = Convert.ToInt64(item.UseCase) }, Action = new ActionDto() { FullTitle = (item.CurrentAction != null ? item.CurrentAction.ActionName : ""), EntityId = Convert.ToInt64(item.Action) } }).ToList();

                    useCase.UseCaseActions = dataDto;
                }
                lstDto.Add(useCase);
            }
            return lstDto;
        }
        public DataLayer.Model.Core.UseCase.UseCase GetEntity(UseCaseDto dto, BusinessParam bp, bool setCreate)
        {
            var useCase = new DataLayer.Model.Core.UseCase.UseCase()
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.UseCaseName + " | " + dto.TableName + " | " + dto.Clazz + " | " + dto.EntityId,
                UseCaseName = dto.UseCaseName,
                VirtualNode = dto.VirtualNode,
                ParentId = dto.Parent?.EntityId,
                TableName = dto.TableName,
                Clazz = dto.Clazz
            };
            return SetMandatoryField(useCase, bp, setCreate);
        }

        public DataLayer.Model.Core.UseCase.UseCase SetMandatoryField(DataLayer.Model.Core.UseCase.UseCase useCase,
            BusinessParam bp, bool setCreate)
        {
            useCase.AccessKey = bp.UserInfo.AccessKey;
            useCase.UpdateBy = bp.UserInfo.UserId;
            useCase.Updated = DateTime.Now;
            if (!setCreate) return useCase;
            useCase.CreateBy = bp.UserInfo.UserId;
            useCase.Created = DateTime.Now;
            return useCase;
        }
    }
}