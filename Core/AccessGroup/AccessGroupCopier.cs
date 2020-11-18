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
        public AccessGroupDto GetDto(DataLayer.Model.Core.AccessGroup.AccessGroup accessGroup)
        {
            var createUser = accessGroup.CreateUserEntity != null
                ? new UserDto() { EntityId = accessGroup.CreateUserEntity.EntityId, Username = accessGroup.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = accessGroup.UpdateUserEntity != null
                ? new UserDto() { EntityId = accessGroup.UpdateUserEntity.EntityId, Username = accessGroup.UpdateUserEntity.Username }
                : new UserDto();
            var data = new AccessGroupDto
            {
                EntityId = accessGroup.EntityId,
                Name = accessGroup.Name,
                Created = Util.GetTimeStamp(accessGroup.Created),
                Updated = Util.GetTimeStamp(accessGroup.Updated),
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Active = accessGroup.Active,
                Code = accessGroup.Code
            };
            var list = new List<UseCaseActionDto>();
            if (accessGroup.ParentAccessGroupUseCaseActionAccessGroup != null)
            {
                foreach (var item in accessGroup.ParentAccessGroupUseCaseActionAccessGroup)
                {
                    var dto = new UseCaseActionDto()
                    {
                        Action = new ActionDto() { EntityId = item.CurrentUseCaseAction.CurrentAction.EntityId, ActionName = item.CurrentUseCaseAction.CurrentAction.ActionName },
                        
                    };
                    if (item.CurrentUseCaseAction?.UseCase != null)
                    {
                        dto.UseCase = new UseCaseDto() {EntityId = item.CurrentUseCaseAction.UseCase.Value};
                    }


                    list.Add(dto);
                }

                data.UseCaseActionList = list;
            }
            return data;
        }
        public List<AccessGroupDto> GetDto(List<DataLayer.Model.Core.AccessGroup.AccessGroup> lstAccessGroup)
        {
            return lstAccessGroup.Select(entity => new AccessGroupDto
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