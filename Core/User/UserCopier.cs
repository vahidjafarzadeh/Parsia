using System;
using DataLayer.Base;
using DataLayer.Model.Core.User;
using DataLayer.Tools;

namespace Parsia.Core.User
{
    public class UserCopier : IBaseCopier<UserDto, Users>
    {
        public UserDto GetDto(Users entity)
        {
            return new UserDto
            {
                EntityId = entity.EntityId,
                Username = entity.Username,
                Password = entity.Password,
                EmailCode = entity.EmailCode,
                PhoneCode = entity.PhoneCode,
                Attempt = entity.Attempt,
                LastVisit = entity.LastVisit
            };
        }

        public Users GetEntity(UserDto dto, BusinessParam bp, bool setCreate)
        {
            var user = new Users
            {
                EntityId = dto.EntityId,
                Username = dto.Username,
                Password = dto.Password,
                EmailCode = dto.EmailCode,
                PhoneCode = dto.PhoneCode,
                Attempt = dto.Attempt,
                LastVisit = dto.LastVisit,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Username + " | " + dto.EntityId
            };
            return SetMandatoryField(user, bp, setCreate);
        }

        public Users SetMandatoryField(Users user, BusinessParam bp, bool setCreate)
        {
            user.UpdateBy = bp.UserInfo.UserId;
            user.Updated = DateTime.Now;
            if (!setCreate) return user;
            user.CreateBy = bp.UserInfo.UserId;
            user.Created = DateTime.Now;
            return user;
        }
    }
}