using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Base;
using DataLayer.Tools;

namespace Parsia.Core.Action
{
    public class ActionCopier : IBaseCopier<ActionDto, DataLayer.Model.Core.Action.Action>
    {
        public ActionDto GetDto(DataLayer.Model.Core.Action.Action entity)
        {
            return new ActionDto
            {
                EntityId = entity.EntityId,
                ActionName = entity.ActionName
            };
        }

        public DataLayer.Model.Core.Action.Action GetEntity(ActionDto dto, BusinessParam bp, bool setCreate)
        {
            var action = new DataLayer.Model.Core.Action.Action
            {
                EntityId = dto.EntityId,
                ActionName = dto.ActionName,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.ActionName + " | " + dto.EntityId
            };
            return SetMandatoryField(action, bp, setCreate);
        }

        public List<ActionDto> GetDto(List<DataLayer.Model.Core.Action.Action> entity)
        {
            return entity.Select(action => new ActionDto
                    {EntityId = action.EntityId, ActionName = action.ActionName, ActionEnName = action.ActionEnName})
                .ToList();
        }

        public DataLayer.Model.Core.Action.Action SetMandatoryField(DataLayer.Model.Core.Action.Action action,
            BusinessParam bp, bool setCreate)
        {
            action.AccessKey = bp.UserInfo.AccessKey;
            action.UpdateBy = bp.UserInfo.UserId;
            action.Updated = DateTime.Now;
            if (!setCreate) return action;
            action.CreateBy = bp.UserInfo.UserId;
            action.Created = DateTime.Now;
            return action;
        }
    }
}