using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Base;
using DataLayer.Coder;
using DataLayer.Context;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Parsia.Core.Elastic;

namespace Parsia.Core.File
{
    public class FileFacade : IBaseFacade<FileDto>
    {
        private static readonly FileFacade Facade = new FileFacade();
        private static readonly FileCopier Copier = new FileCopier();
        private readonly ParsiContext _context = new ParsiContext();
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            try
            {
                var queryString = "select * from (SELECT * FROM [CO].[File]) e" +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, "File", false, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                var files = _context.File.FromSqlRaw(queryString).OrderBy(x => x.Extension)
                    .ThenByDescending(x => x.Created).ToList();
                var lstData = files.Select(file => Copier.GetDto(file)).ToList();
                return lstData.Count <= 0
                    ? new ServiceResult<object>(new List<FileDto>(), 0)
                    : new ServiceResult<object>(lstData, lstData.Count);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "FileFacade.GridView", bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, FileDto dto)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> ShowRow(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> Delete(BusinessParam bp)
        {
            try
            {
                long entityId = 0;
                if (bp.Clause?.Wheres != null && bp.Clause.Wheres.Count > 0)
                    foreach (var item in bp.Clause.Wheres.Where(item => item.Key.Equals("entityId")))
                        entityId = long.Parse(item.Value);

                var record = _unitOfWork.File.GetRecord(entityId);
                if (record == null)
                    return new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "فایل مورد نظر یافت نشد");
                record.Deleted = record.EntityId;
                _unitOfWork.File.Update(record);
                _unitOfWork.File.Save();
                return new ServiceResult<object>(Copier.GetDto(record), 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "FileFacade.Delete", bp.UserInfo);
            }
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> GetAllExtension(BusinessParam bp)
        {
            try
            {
                var extensionList = _unitOfWork.File.Get().Select(p => p.Extension).Distinct().ToList();
                return extensionList.Count <= 0
                    ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound,
                        "پسوندی برای نمایش در قسمت فیلترها یافت نشد یافت نشد")
                    : new ServiceResult<object>(extensionList, extensionList.Count);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "FileFacade.GetAllExtension", bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDetails(BusinessParam bp)
        {
            try
            {
                long entityId = 0;
                if (bp.Clause?.Wheres != null && bp.Clause.Wheres.Count > 0)
                    foreach (var item in bp.Clause.Wheres.Where(item => item.Key.Equals("entityId")))
                        entityId = long.Parse(item.Value);

                var fileDto = GetFileFromDataBaseWithId(entityId, bp);
                if (fileDto == null)
                    return new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "فایل مورد نظر یافت نشد");
                var pathOfFile =
                    GetPathOfFile(
                        new JsonFileDto
                            {EntityId = fileDto.EntityId, Path = fileDto.Path, Extension = fileDto.Extension}, bp);
                if (string.IsNullOrEmpty(pathOfFile))
                    return new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "فایل مورد نظر یافت نشد");

                fileDto.Path = pathOfFile;
                return new ServiceResult<object>(fileDto, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "FileFacade.GetDetails", bp.UserInfo);
            }
        }

        public ServiceResult<object> CreateFolder(BusinessParam bp, FolderDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.FolderName))
                    return ExceptionUtil.ExceptionHandler("لطفا نام پوشه را وارد نمایید", "FileFacade.CreateFolder",
                        bp.UserInfo);
                long? parentId = null;
                var webRootPath = bp.Environment.WebRootPath;
                var fileName = "\\file\\" + DateTime.UtcNow.RepositoryFolderName();
                if (!string.IsNullOrEmpty(dto.Path))
                {
                    parentId = dto.ParentId;
                    fileName = dto.Path + DateTime.UtcNow.RepositoryFolderName();
                }

                var exists = Directory.Exists(webRootPath + fileName);
                if (exists)
                    return ExceptionUtil.ExceptionHandler("نام پوشه تکراری می باشد", "FileFacade.CreateFolder",
                        bp.UserInfo);
                Directory.CreateDirectory(webRootPath + fileName);
                var fileDto = new FileDto
                {
                    Alt = "",
                    DataSize = "0",
                    Description = "",
                    DisplayInFileManager = true,
                    Extension = null,
                    FullTitle = fileName + " | " + dto.FolderName,
                    Name = dto.FolderName,
                    ParentId = parentId,
                    Path = fileName + "\\",
                    Thumbnail = fileName + "\\",
                    Title = dto.FolderName
                };
                var file = Copier.GetEntity(fileDto, bp, true);
                var done = _unitOfWork.File.Insert(file);
                _unitOfWork.File.Save();
                if (!done)
                    return ExceptionUtil.ExceptionHandler("خطا در ذخیره فایل درون دیتابیس", "FileFacade.CreateFolder",
                        bp.UserInfo);
                Elastic<FileDto, DataLayer.Model.Core.File.File>.SaveToHistory(fileDto, file, file.EntityId, bp);
                Elastic<FileDto, DataLayer.Model.Core.File.File>.SaveToElastic(fileDto, "File", file.EntityId, bp);
                fileDto.EntityId = file.EntityId;
                return new ServiceResult<object>(fileDto, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "FileFacade.CreateFolder", bp.UserInfo);
            }
        }

        public async Task<ServiceResult<object>> CreateFile(BusinessParam bp, HttpRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Form["name"]))
                    return ExceptionUtil.ExceptionHandler("لطفا نام فایل را وارد نمایید", "FileFacade.CreateFile",
                        bp.UserInfo);
                if (request.Form.Files["file"] == null)
                    return ExceptionUtil.ExceptionHandler("لطفا فایل را انتخاب نمایید", "FileFacade.CreateFile",
                        bp.UserInfo);


                long? parentId = null;
                var webRootPath = bp.Environment.WebRootPath;
                var fileName = "\\file\\" + DateTime.UtcNow.RepositoryFolderName();
                if (!string.IsNullOrEmpty(request.Form["path"]))
                {
                    parentId = long.Parse(request.Form["parentId"]);
                    fileName = request.Form["path"] + DateTime.UtcNow.RepositoryFolderName();
                }

                var formFile = request.Form.Files["file"];
                if (formFile.Length <= 0)
                    return ExceptionUtil.ExceptionHandler("فایل ارسالی خالی می باشد", "FileFacade.CreateFile",
                        bp.UserInfo);
                var extension = formFile.FileName.Split(".")[1];
                var exists = System.IO.File.Exists(webRootPath + fileName + "." + extension);
                if (exists)
                    return ExceptionUtil.ExceptionHandler("فایل تکراری می باشد", "FileFacade.CreateFile",
                        bp.UserInfo);
                var checkFileIsValid = CheckFileIsValid(formFile, extension);
                if (!checkFileIsValid.Done)
                    return checkFileIsValid;
                using (var inputStream = new FileStream(webRootPath + fileName + "." + extension, FileMode.Create))
                {
                    await formFile.CopyToAsync(inputStream);
                    var array = new byte[inputStream.Length];
                    inputStream.Seek(0, SeekOrigin.Begin);
                    inputStream.Read(array, 0, array.Length);
                }

                var fileDto = new FileDto
                {
                    Alt = request.Form["alt"],
                    DataSize = formFile.Length.ToString(),
                    Description = request.Form["description"],
                    DisplayInFileManager = true,
                    Extension = extension.ToLower(),
                    FullTitle =
                        request.Form["name"] + " | " + request.Form["description"] + " | " + request.Form["alt"],
                    Name = request.Form["name"],
                    ParentId = parentId,
                    Path = fileName + "." + extension,
                    Thumbnail = fileName + "_thumb." + extension,
                    Title = formFile.FileName
                };
                if (formFile.IsImage())
                {
                    var thumbnail = CreateThumbnail(formFile, fileDto, bp);
                }

                var file = Copier.GetEntity(fileDto, bp, true);
                var done = _unitOfWork.File.Insert(file);
                _unitOfWork.File.Save();
                if (!done)
                    return ExceptionUtil.ExceptionHandler("خطا در ذخیره فایل درون دیتابیس", "FileFacade.CreateFile",
                        bp.UserInfo);
                Elastic<FileDto, DataLayer.Model.Core.File.File>.SaveToHistory(fileDto, file, file.EntityId, bp);
                Elastic<FileDto, DataLayer.Model.Core.File.File>.SaveToElastic(fileDto, "File", file.EntityId, bp);
                fileDto.EntityId = file.EntityId;
                return new ServiceResult<object>(fileDto, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "FileFacade.CreateFile", bp.UserInfo);
            }
        }

        public static FileFacade GetInstance()
        {
            return Facade;
        }

        private FileDto GetFileFromDataBaseWithId(long entityId, BusinessParam bp)
        {
            try
            {
                var file = _unitOfWork.File.GetRecord(entityId);
                return Copier.GetDto(file);
            }
            catch (Exception ex)
            {
                if (bp != null) ExceptionUtil.ExceptionHandler(ex, "FileFacade.GetFileFromDataBaseWithId", bp.UserInfo);
                return null;
            }
        }

        private ServiceResult<object> CheckFileIsValid(IFormFile file, string extension)
        {
            try
            {
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception ex)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "");
            }
        }

        private async Task<ServiceResult<object>> CreateThumbnail(IFormFile formFile, FileDto fileDto, BusinessParam bp)
        {
            try
            {
                var webRootPath = bp.Environment.WebRootPath;
                var dto = new FileDto
                {
                    Alt = fileDto.Alt,
                    DataSize = fileDto.DataSize,
                    Description = fileDto.Description,
                    DisplayInFileManager = false,
                    Extension = fileDto.Extension,
                    FullTitle = fileDto.FullTitle,
                    Name = fileDto.Name,
                    ParentId = fileDto.ParentId,
                    Path = fileDto.Path,
                    Thumbnail = fileDto.Thumbnail,
                    Title = fileDto.Title
                };
                var resize = new ImageResize();
                resize.Resize(webRootPath + dto.Path, webRootPath + dto.Thumbnail);
                dto.DisplayInFileManager = false;
                dto.Path = dto.Thumbnail;
                var file = Copier.GetEntity(dto, bp, true);
                var done = _unitOfWork.File.Insert(file);
                _unitOfWork.File.Save();
                if (!done)
                    return ExceptionUtil.ExceptionHandler("خطا در ذخیره فایل درون دیتابیس", "FileFacade.CreateFile",
                        bp.UserInfo);
                Elastic<FileDto, DataLayer.Model.Core.File.File>.SaveToHistory(dto, file, file.EntityId, bp);
                Elastic<FileDto, DataLayer.Model.Core.File.File>.SaveToElastic(dto, "File", file.EntityId, bp);
                dto.EntityId = file.EntityId;
                return new ServiceResult<object>(dto, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "FileFacade.CreateThumbnail", bp.UserInfo);
            }
        }

        private string GetPathOfFile(JsonFileDto dto, BusinessParam bp)
        {
            try
            {
                return
                    $"/service/file/download/{FileRepositoryFacade.ReplaceInValidCharacter(Encryption.Encrypt(GetJsonData(dto)))}.{dto.Extension}";
            }
            catch (Exception ex)
            {
                ExceptionUtil.ExceptionHandler(ex, "FileFacade.GetDetails", bp.UserInfo);
                return "";
            }
        }

        private string GetJsonData(JsonFileDto dto)
        {
            return JsonConvert.SerializeObject(dto);
        }

        private JsonFileDto GetJsonDto(string data)
        {
            return JsonConvert.DeserializeObject<JsonFileDto>(data);
        }

        public async Task<IActionResult> Download(string file, BusinessParam bp, bool tumbnail)
        {
            try
            {
                var lastPoint = file.LastIndexOf(".", StringComparison.Ordinal);
                var link = file.Substring(0, lastPoint);
                var jsonFileDto = GetJsonDto(Encryption.Decryption(FileRepositoryFacade.ReplaceValidCharacter(link)));
                var fileDto = GetFileFromDataBaseWithId(jsonFileDto.EntityId, null);
                var path = bp.Environment.WebRootPath + fileDto.Path;
                if (tumbnail)
                {
                    var lastIndexOf = path.LastIndexOf(".", StringComparison.Ordinal);
                    var substring = path.Substring(0, lastIndexOf);
                    substring += "_thumb" + path.Substring(lastIndexOf);
                    path = substring;
                }

                var stream = new FileStream(path, FileMode.Open);
                return new FileStreamResult(stream, FileRepositoryFacade.MimType[fileDto.Extension])
                    {FileDownloadName = fileDto.Name + "." + fileDto.Extension};
            }
            catch (Exception e)
            {
                ExceptionUtil.ExceptionHandler(e, "FileFacade.Download", null);
                return null;
            }
        }
    }
}