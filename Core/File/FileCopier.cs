using System;
using DataLayer.Base;
using DataLayer.Tools;

namespace Parsia.Core.File
{
    public class FileCopier:IBaseCopier<FileDto,DataLayer.Model.Core.File.File>
    {
        public FileDto GetDto(DataLayer.Model.Core.File.File entity)
        {
            return new FileDto
            {
                EntityId = entity.EntityId,
                Name = entity.Name,
                ParentId = entity.ParentId,
                Alt = entity.Alt,
                DataSize = entity.DataSize,
                Description = entity.Description,
                DisplayInFileManager = entity.DisplayInFileManager,
                Extension = entity.Extension,
                Path = entity.Path,
                Thumbnail = entity.Thumbnail,
                Title = entity.Title
            };
        }

        public DataLayer.Model.Core.File.File GetEntity(FileDto dto, BusinessParam bp, bool setCreate)
        {
            var file = new DataLayer.Model.Core.File.File
            {
                EntityId = dto.EntityId,
                Active = dto.Active,
                Code = dto.Code,
                Deleted = dto.Deleted,
                FullTitle = dto.Name + " | " +dto.Description + " | " + dto.Title + " | " + dto.EntityId + " | " + dto.Alt+" | "+dto.Extension,
                Name = dto.Name,
                ParentId = dto.ParentId,
                DisplayInFileManager = dto.DisplayInFileManager,
                Description = dto.Description,
                Extension = dto.Extension,
                Path = dto.Path,
                Thumbnail = dto.Thumbnail,
                Title = dto.Title,
                Alt = dto.Alt,
                DataSize = dto.DataSize
            };
            return SetMandatoryField(file, bp, setCreate);
        }
        public DataLayer.Model.Core.File.File SetMandatoryField(DataLayer.Model.Core.File.File file, BusinessParam bp, bool setCreate)
        {
            file.UpdateBy = bp.UserInfo.UserId;
            file.Updated = DateTime.Now;
            if (!setCreate) return file;
            file.CreateBy = bp.UserInfo.UserId;
            file.Created = DateTime.Now;
            return file;
        }
    }
}
