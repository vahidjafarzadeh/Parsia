using System;
using DataLayer.Base;
using DataLayer.Tools;

namespace Parsia.Core.AccessGroup
{
    public class AccessGroupCopier : IBaseCopier<AccessGroupDto, DataLayer.Model.Core.AccessGroup.AccessGroup>
    {
        public AccessGroupDto GetDto(DataLayer.Model.Core.AccessGroup.AccessGroup entity)
        {
            return new AccessGroupDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name
            };
        }

        public DataLayer.Model.Core.AccessGroup.AccessGroup GetEntity(AccessGroupDto dto, BusinessParam bp, bool setCreate)
        {
            var accessGroup = new DataLayer.Model.Core.AccessGroup.AccessGroup
            {
                EntityId = dto.EntityId,
                Name = dto.Name,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Name + " | " + dto.EntityId
            };
            return SetMandatoryField(accessGroup, bp, setCreate);
        }
        public DataLayer.Model.Core.AccessGroup.AccessGroup SetMandatoryField(DataLayer.Model.Core.AccessGroup.AccessGroup accessGroup, BusinessParam bp, bool setCreate)
        {
            accessGroup.UpdateBy = bp.UserInfo.UserId;
            accessGroup.Updated = DateTime.Now;
            if (!setCreate) return accessGroup;
            accessGroup.CreateBy = bp.UserInfo.UserId;
            accessGroup.Created = DateTime.Now;
            return accessGroup;
        }
    }
}
