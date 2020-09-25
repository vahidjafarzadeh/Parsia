using System;
using DataLayer.Base;
using DataLayer.Tools;

namespace Parsia.Core.Location
{
    public class LocationCopier : IBaseCopier<LocationDto, DataLayer.Model.Core.Location.Location>
    {
        public LocationDto GetDto(DataLayer.Model.Core.Location.Location entity)
        {
            return new LocationDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                ParentId = entity.ParentId
            };
        }

        public DataLayer.Model.Core.Location.Location GetEntity(LocationDto dto, BusinessParam bp, bool setCreate)
        {
            var location = new DataLayer.Model.Core.Location.Location
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Name + " | " + dto.EntityId,
                Name = dto.Name,
                ParentId = dto.ParentId
            };
            return SetMandatoryField(location, bp, setCreate);
        }

        public DataLayer.Model.Core.Location.Location SetMandatoryField(DataLayer.Model.Core.Location.Location location,
            BusinessParam bp, bool setCreate)
        {
            location.UpdateBy = bp.UserInfo.UserId;
            location.Updated = DateTime.Now;
            if (!setCreate) return location;
            location.CreateBy = bp.UserInfo.UserId;
            location.Created = DateTime.Now;
            return location;
        }
    }
}