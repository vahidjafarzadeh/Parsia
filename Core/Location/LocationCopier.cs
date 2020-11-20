using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.ComboVal;

namespace Parsia.Core.Location
{
    public class LocationCopier : IBaseCopier<LocationDto, DataLayer.Model.Core.Location.Location>
    {

        public LocationDto GetDtDataLayer.Model.Core.Location.Locationon entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            var location = new LocationDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                Type = new ComboValDto() { EntityId = entity.Type},
                Active = entity.Active,
                Code = entity.Code,
                FullTitle = entity.FullTitle,
                Deleted = entity.Deleted,
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated)
            };
            if (entity.CurrentLocation != null)
            {
                location.Parent = new LocationDto(){EntityId = entity.CurrentLocation.EntityId,Name = entity.CurrentLocation.Name,FullTitle = entity.CurrentLocation.FullTitle};
            }

            if (entity.CurrentType != null)
            {
                location.Type.Name = entity.CurrentType.Name;
            }

            return location;
        }

    DataLayer.Model.Core.Location.Locationon.Location GetEntity(LocationDto dto, BusinessParam bp, bool setCreate)
        {
            var locaDataLayer.Model.Core.Location.Locationon.Location
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = LocationFacade.GetInstance().GetParent(bp,dto.Parent?.EntityId) + " - " + dto.Name ,
                Name = dto.Name,
                ParentId = dto.Parent?.EntityId,
                Type = dto.Type.EntityId
            };
            return SetMandatoryField(location, bp, setCreate);
        }

    DataLayer.Model.Core.Location.Locationon.Location SetMandDataLayer.Model.Core.Location.Locationon.Location location,
            BusinessParam bp, bool setCreate)
        {
            location.AccessKey = bp.UserInfo.AccessKey;
            location.UpdateBy = bp.UserInfo.UserId;
            location.Updated = DateTime.Now;
            if (!setCreate) return location;
            location.CreateBy = bp.UserInfo.UserId;
            location.Created = DateTime.Now;
            return location;
        }
    }
}