using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.Action;
using Parsia.Core.UseCase;
using Parsia.Core.UseCaseAction;

namespace Parsia.Core.AccessGroup
{
    public class AccessGroupCopier : IBaseCopier<AccessGroupDto, DataLayer.Model.Core.AccessGroup.AccessGroup>
    {
        public AccessGroupDto GetDto(DataLayer.Model.Core.AccessGroup.AccessGroup entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            var data = new AccessGroupDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Active = entity.Active,
                Code = entity.Code
            };
            var list = new List<UseCaseActionDto>();
            if (entity.ParentAccessGroupUseCaseActionAccessGroup != null)
            {
                foreach (var item in entity.ParentAccessGroupUseCaseActionAccessGroup)
                {
                    var dto = new UseCaseActionDto()
                    {
                        Action = new ActionDto() { EntityId = item.CurrentUseCaseAction.CurrentAction.EntityId, ActionName = item.CurrentUseCaseAction.CurrentAction.ActionName },
                        UseCase = new UseCaseDto() { EntityId = item.CurrentUseCaseAction.UseCase.Value }
                    };
                    list.Add(dto);
                }

                data.UseCaseActionList = list;
            }
            return data;
        }
        public List<AccessGroupDto> GetDto(List<DataLayer.Model.Core.AccessGroup.AccessGroup> lstData)
        {
            return lstData.Select(entity => new AccessGroupDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                Active = entity.Active,
                Code = entity.Code
            }).ToList();
        }
        public DataLayer.Model.Core.AccessGroup.AccessGroup GetEntity(AccessGroupDto dto, BusinessParam bp,
            bool setCreate)
        {
            var accessGroup = new DataLayer.Model.Core.AccessGroup.AccessGroup
            {
                EntityId = dto.EntityId,
                Name = dto.Name,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Name + " | " + dto.EntityId,
            };
            return SetMandatoryField(accessGroup, bp, setCreate);
        }

        public DataLayer.Model.Core.AccessGroup.AccessGroup SetMandatoryField(
            DataLayer.Model.Core.AccessGroup.AccessGroup accessGroup, BusinessParam bp, bool setCreate)
        {
            accessGroup.AccessKey = bp.UserInfo.AccessKey;
            accessGroup.UpdateBy = bp.UserInfo.UserId;
            accessGroup.Updated = DateTime.Now;
            if (!setCreate) return accessGroup;
            accessGroup.CreateBy = bp.UserInfo.UserId;
            accessGroup.Created = DateTime.Now;
            return accessGroup;
        }
    }
}