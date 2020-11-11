﻿using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.User;

namespace Parsia.Core.ComboVal
{
    public class ComboValCopier : IBaseCopier<ComboValDto, DataLayer.Model.Core.ComboVal.ComboVal>
    {
        public ComboValDto GetDto(DataLayer.Model.Core.ComboVal.ComboVal entity)
        {
            var userCopier = new UserCopier();
            var createUser = entity.CreateUserEntity != null
                ? userCopier.GetDto(entity.CreateUserEntity)
                : new UserDto();
            var updateUser = entity.CreateUserEntity != null
                ? userCopier.GetDto(entity.CreateUserEntity)
                : new UserDto();
            var comboValDto = new ComboValDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                AdminOnly = entity.AdminOnly,
                Value = entity.Value,
                Created = entity.Created?.GetTimeStamp(),
                Updated = entity.Updated?.GetTimeStamp(),
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Active = entity.Active,
                Code = entity.Code
            };
            if (entity.CurrentComboVal != null)
                comboValDto.Parent = new ComboValDto
                {
                    EntityId = entity.CurrentComboVal.EntityId,
                    Name = entity.CurrentComboVal?.Name
                };
            return comboValDto;
        }

        public DataLayer.Model.Core.ComboVal.ComboVal GetEntity(ComboValDto dto, BusinessParam bp, bool setCreate)
        {
            var comboVal = new DataLayer.Model.Core.ComboVal.ComboVal
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Name + " | " + dto.Value + " | " + dto.EntityId + " | " + dto.Parent,
                Name = dto.Name,
                AdminOnly = dto.AdminOnly,
                ParentId = dto.Parent?.EntityId,
                Value = dto.Value
            };
            return SetMandatoryField(comboVal, bp, setCreate);
        }

        public DataLayer.Model.Core.ComboVal.ComboVal SetMandatoryField(DataLayer.Model.Core.ComboVal.ComboVal comboVal,
            BusinessParam bp, bool setCreate)
        {
            comboVal.UpdateBy = bp.UserInfo.UserId;
            comboVal.Updated = DateTime.Now;
            if (!setCreate) return comboVal;
            comboVal.CreateBy = bp.UserInfo.UserId;
            comboVal.Created = DateTime.Now;
            return comboVal;
        }
    }
}