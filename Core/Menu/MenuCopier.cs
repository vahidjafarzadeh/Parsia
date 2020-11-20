using System;
using DataLayer.Base;
using DataLayer.Tools;
using Parsia.Core.ComboVal;
using Parsia.Core.File;
using Parsia.Core.UseCase;

namespace Parsia.Core.Menu
{
    public class MenuCopier : IBaseCopier<MenuDto, DataLayer.Model.Core.Menu.Menu>
    {
        public MenuDto GetDto(DataLayer.Model.Core.Menu.Menu entity)
        {
            var createUser = entity.CreateUserEntity != null
                ? new UserDto() { EntityId = entity.CreateUserEntity.EntityId, Username = entity.CreateUserEntity.Username }
                : new UserDto();
            var updateUser = entity.UpdateUserEntity != null
                ? new UserDto() { EntityId = entity.UpdateUserEntity.EntityId, Username = entity.UpdateUserEntity.Username }
                : new UserDto();
            var dto = new MenuDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                Path = entity.Path,
                Title = entity.Title,
                Icon = entity.Icon,
                OrderNode = entity.OrderNode,
                Active = entity.Active,
                Code = entity.Code,
                FullTitle = entity.FullTitle,
                Deleted = entity.Deleted,
                CreatedBy = createUser,
                UpdatedBy = updateUser,
                Created = Util.GetTimeStamp(entity.Created),
                Updated = Util.GetTimeStamp(entity.Updated)
            };
            if (entity.CurrentFile != null)
            {
                dto.File = new FileDto()
                {
                    EntityId = entity.CurrentFile.EntityId,
                    Description = entity.CurrentFile.Description,
                    Thumbnail = entity.CurrentFile.Thumbnail,
                    Name = entity.CurrentFile.Name
                };
            }

            if (entity.CurrentMenu != null)
            {
                dto.Parent = new MenuDto()
                {
                    EntityId = entity.CurrentMenu.EntityId,
                    Name = entity.CurrentMenu.Name,
                    Title = entity.CurrentMenu.Title
                };
            }

            if (entity.CurrentTargetComboVal != null)
            {
                dto.Target = new ComboValDto()
                {
                    EntityId = entity.CurrentTargetComboVal.EntityId,
                    Name = entity.CurrentTargetComboVal.Name,
                    FullTitle = entity.CurrentTargetComboVal.FullTitle
                };
            }

            if (entity.CurrentUseCase != null)
            {
                dto.UseCase = new UseCaseDto()
                {
                    EntityId = entity.CurrentUseCase.EntityId,
                    UseCaseName = entity.CurrentUseCase.UseCaseName,
                    FullTitle = entity.CurrentUseCase.FullTitle
                };
            }
            return dto;

        }

        public DataLayer.Model.Core.Menu.Menu GetEntity(MenuDto dto, BusinessParam bp, bool setCreate)
        {
            var menu = new DataLayer.Model.Core.Menu.Menu
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Name + " | " + dto.EntityId + " | " + dto.Title,
                Name = dto.Name,
                File = dto.File?.EntityId,
                UseCase = dto.UseCase.EntityId,
                Title = dto.Title,
                Path = dto.Path,
                Icon = dto.Icon,
                Target = dto.Target.EntityId,
                OrderNode = dto.OrderNode,
                ParentId = dto.Parent?.EntityId
            };
            return SetMandatoryField(menu, bp, setCreate);
        }

        public DataLayer.Model.Core.Menu.Menu SetMandatoryField(DataLayer.Model.Core.Menu.Menu menu, BusinessParam bp,
            bool setCreate)
        {
            menu.AccessKey = bp.UserInfo.AccessKey;
            menu.UpdateBy = bp.UserInfo.UserId;
            menu.Updated = DateTime.Now;
            if (!setCreate) return menu;
            menu.CreateBy = bp.UserInfo.UserId;
            menu.Created = DateTime.Now;
            return menu;
        }
    }
}