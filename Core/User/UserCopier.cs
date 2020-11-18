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
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            return new UserDto
            {
                EntityId = entity.EntityId,
                Username = entity.Username,
                Password = entity.Password,
                EmailCode = entity.EmailCode,
                PhoneCode = entity.PhoneCode,
                Attempt = entity.Attempt,
                LastVisit = entity.LastVisit,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated),
                Active = entity.Active,
                Deleted = entity.Deleted,
                CreatedBy = createUser,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PersonId = entity.PersonId,
                UpdatedBy = updateUser,
                Code = entity.Code,
                IsAdmin = entity.IsAdmin
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
                FullTitle = dto.Username + " | " + dto.FirstName+" | "+ dto.LastName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonId = dto.PersonId,
                IsAdmin = dto.IsAdmin
            };
            if (string.IsNullOrEmpty(user.Code))
            {
                user.Code = "-";
            }
            return SetMandatoryField(user, bp, setCreate);
        }

        public Users SetMandatoryField(Users user, BusinessParam bp, bool setCreate)
        {
            user.AccessKey = bp.UserInfo.AccessKey;
            user.UpdateBy = bp.UserInfo.UserId;
            user.Updated = DateTime.Now;
            if (!setCreate) return user;
            user.CreateBy = bp.UserInfo.UserId;
            user.Created = DateTime.Now;
            return user;
        }
    }
}