using System;
using DataLayer.Base;
using DataLayer.Tools;

namespace Parsia.Core.ComboVal
{
    public class ComboValCopier : IBaseCopier<ComboValDto, DataLayer.Model.Core.ComboVal.ComboVal>
    {
        public ComboValDto GetDto(DataLayer.Model.Core.ComboVal.ComboVal entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            var comboValDto = new ComboValDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                AdminOnly = entity.AdminOnly,
                Value = entity.Value,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
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

       DataLayer.Model.Core.ComboVal.ComboValComboVal GetEntity(ComboValDto dto, BusinessParam bp, bool setCreate)
        {
            var comboVaDataLayer.Model.Core.ComboVal.ComboValComboVal
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Name + " | " + dto.Value + " | " + dto.Code + " | " + dto.EntityId,
                Name = dto.Name,
                AdminOnly = dto.AdminOnly,
                ParentId = dto.Parent?.EntityId,
                Value = dto.Value
            };
            return SetMandatoryField(comboVal, bp, setCreate);
        }

       DataLayer.Model.Core.ComboVal.ComboValComboVal SetMandatoDataLayer.Model.Core.ComboVal.ComboValComboVal comboVal,
            BusinessParam bp, bool setCreate)
        {
            comboVal.AccessKey = bp.UserInfo.AccessKey;
            comboVal.UpdateBy = bp.UserInfo.UserId;
            comboVal.Updated = DateTime.Now;
            if (!setCreate) return comboVal;
            comboVal.CreateBy = bp.UserInfo.UserId;
            comboVal.Created = DateTime.Now;
            return comboVal;
 
     }
    }
}