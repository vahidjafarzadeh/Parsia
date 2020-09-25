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
            return new MenuDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                ParentId = entity.ParentId,
                File = new FileDto() { EntityId = entity.CurrentFile.EntityId},
                UseCase = new UseCaseDto() { EntityId = entity.CurrentUseCase.EntityId},
                Path = entity.Path,
                Title = entity.Title,
                Icon = entity.Icon,
                OrderNode = entity.OrderNode,
                Target = new ComboValDto() { EntityId = entity.Target}
            };
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
                ParentId = dto.ParentId,
                File = dto.File.EntityId,
                UseCase = dto.UseCase.EntityId,
                Title = dto.Title,
                Path = dto.Path,
                Icon = dto.Icon,
                Target = dto.Target.EntityId,
                OrderNode = dto.OrderNode
            };
            return SetMandatoryField(menu, bp, setCreate);
        }
        public DataLayer.Model.Core.Menu.Menu SetMandatoryField(DataLayer.Model.Core.Menu.Menu menu, BusinessParam bp, bool setCreate)
        {
            menu.UpdateBy = bp.UserInfo.UserId;
            menu.Updated = DateTime.Now;
            if (!setCreate) return menu;
            menu.CreateBy = bp.UserInfo.UserId;
            menu.Created = DateTime.Now;
            return menu;
        }
    }
}
