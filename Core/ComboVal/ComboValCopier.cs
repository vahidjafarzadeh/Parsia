using System;
using DataLayer.Base;
using DataLayer.Tools;

namespace Parsia.Core.ComboVal
{
    public class ComboValCopier : IBaseCopier<ComboValDto, DataLayer.Model.Core.ComboVal.ComboVal>
    {
        public ComboValDto GetDto(DataLayer.Model.Core.ComboVal.ComboVal entity)
        {
            return new ComboValDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                AdminOnly = entity.AdminOnly,
                ParentId = entity.ParentId,
                Value = entity.Value
            };
        }

        public DataLayer.Model.Core.ComboVal.ComboVal GetEntity(ComboValDto dto, BusinessParam bp, bool setCreate)
        {
            var comboVal = new DataLayer.Model.Core.ComboVal.ComboVal
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Name + " | " + dto.Value + " | " + dto.EntityId + " | " + dto.ParentId,
                Name = dto.Name,
                AdminOnly = dto.AdminOnly,
                ParentId = dto.ParentId,
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