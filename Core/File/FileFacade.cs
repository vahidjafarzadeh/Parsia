using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [ClassDetails(Clazz = "File", Facade = "FileFacade")]
    public class FileFacade : IBaseFacade<FileDto>
    {
        private static readonly FileFacade Facade = new FileFacade();
        private static readonly FileCopier Copier = new FileCopier();

        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(FileFacade).GetCustomAttributes(typeof(ClassDetails), true);

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.File.File>();
                var queryString = $"select * from (SELECT * FROM {tableName}) e" +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false,
                                          true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);

                using (var content = new ParsiContext())
                {
                    var files = content.File.FromSqlRaw(queryString).OrderBy(x => x.Extension)
                        .ThenByDescending(x => x.Created).IgnoreQueryFilters().ToList();
                    var lstData = files.Select(file => Copier.GetDto(file)).ToList();
                    return lstData.Count <= 0
                        ? new ServiceResult<object>(new List<FileDto>(), 0)
                        : new ServiceResult<object>(lstData, lstData.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
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
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                long entityId = 0;
                if (bp.Clause?.Wheres != null && bp.Clause.Wheres.Count > 0)
                    foreach (var item in bp.Clause.Wheres.Where(item => item.Key.Equals("entityId")))
                        entityId = long.Parse(item.Value);
                using (var unitOfWork = new UnitOfWork())
                {
                    var record = unitOfWork.File.GetRecord(entityId);
                    if (record == null)
                        return new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "فایل مورد نظر یافت نشد");
                    record.Deleted = record.EntityId;
                    unitOfWork.File.Update(record);
                    unitOfWork.File.Save();
                    return new ServiceResult<object>(Copier.GetDto(record), 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> GetAllExtension(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var extensionList = unitOfWork.File.Get().Select(p => p.Extension).Distinct().ToList();
                    return extensionList.Count <= 0
                        ? new ServiceResult<object>(0, 1)
                        : new ServiceResult<object>(extensionList, extensionList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDetails(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
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
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> CreateFolder(BusinessParam bp, FolderDto dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                if (string.IsNullOrEmpty(dto.FolderName))
                    return ExceptionUtil.ExceptionHandler("لطفا نام پوشه را وارد نمایید",
                        ClassDetails[0].Facade + methodName,
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
                    return ExceptionUtil.ExceptionHandler("نام پوشه تکراری می باشد",
                        ClassDetails[0].Facade + methodName,
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
                using (var unitOfWork = new UnitOfWork())
                {
                    var done = unitOfWork.File.Insert(file);
                    unitOfWork.File.Save();
                    if (!done)
                        return ExceptionUtil.ExceptionHandler("خطا در ذخیره فایل درون دیتابیس",
                            ClassDetails[0].Facade + methodName,
                            bp.UserInfo);
                    Elastic<FileDto, DataLayer.Model.Core.File.File>.SaveToElastic(file, ClassDetails[0].Clazz, bp);
                    fileDto.EntityId = file.EntityId;
                    return new ServiceResult<object>(fileDto, 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public async Task<ServiceResult<object>> CreateFile(BusinessParam bp, HttpRequest request)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                if (string.IsNullOrEmpty(request.Form["name"]))
                    return ExceptionUtil.ExceptionHandler("لطفا نام فایل را وارد نمایید",
                        ClassDetails[0].Facade + methodName,
                        bp.UserInfo);
                if (request.Form.Files["file"] == null)
                    return ExceptionUtil.ExceptionHandler("لطفا فایل را انتخاب نمایید",
                        ClassDetails[0].Facade + methodName,
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
                    return ExceptionUtil.ExceptionHandler("فایل ارسالی خالی می باشد",
                        ClassDetails[0].Facade + methodName,
                        bp.UserInfo);
                var extension = formFile.FileName.Split(".")[1];
                var exists = System.IO.File.Exists(webRootPath + fileName + "." + extension);
                if (exists)
                    return ExceptionUtil.ExceptionHandler("فایل تکراری می باشد", ClassDetails[0].Facade + methodName,
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
                using (var unitOfWork = new UnitOfWork())
                {
                    var done = unitOfWork.File.Insert(file);
                    unitOfWork.File.Save();
                    if (!done)
                        return ExceptionUtil.ExceptionHandler("خطا در ذخیره فایل درون دیتابیس",
                            ClassDetails[0].Facade + methodName,
                            bp.UserInfo);
                    Elastic<FileDto, DataLayer.Model.Core.File.File>.SaveToElastic(file, ClassDetails[0].Clazz, bp);
                    fileDto.EntityId = file.EntityId;
                    return new ServiceResult<object>(fileDto, 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public static FileFacade GetInstance()
        {
            return Facade;
        }

        private FileDto GetFileFromDataBaseWithId(long entityId, BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var file = unitOfWork.File.GetRecord(entityId);
                    return Copier.GetDto(file);
                }
            }
            catch (Exception ex)
            {
                if (bp != null) ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
                return null;
            }
        }

        private ServiceResult<object> CheckFileIsValid(IFormFile file, string extension)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
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
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
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
                using (var unitOfWork = new UnitOfWork())
                {
                    var done = unitOfWork.File.Insert(file);
                    unitOfWork.File.Save();
                    if (!done)
                        return ExceptionUtil.ExceptionHandler("خطا در ذخیره فایل درون دیتابیس",
                            ClassDetails[0].Facade + methodName,
                            bp.UserInfo);
                    Elastic<FileDto, DataLayer.Model.Core.File.File>.SaveToElastic(file, ClassDetails[0].Clazz, bp);
                    dto.EntityId = file.EntityId;
                    return new ServiceResult<object>(dto, 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        private string GetPathOfFile(JsonFileDto dto, BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                return
                    $"/service/file/download/{FileRepositoryFacade.ReplaceInValidCharacter(Encryption.Encrypt(GetJsonData(dto)))}.{dto.Extension}";
            }
            catch (Exception ex)
            {
                ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
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

        public async Task<IActionResult> Download(string file, BusinessParam bp, bool thumbnail)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var lastPoint = file.LastIndexOf(".", StringComparison.Ordinal);
                var link = file.Substring(0, lastPoint);
                var jsonFileDto = GetJsonDto(Encryption.Decryption(FileRepositoryFacade.ReplaceValidCharacter(link)));
                var fileDto = GetFileFromDataBaseWithId(jsonFileDto.EntityId, null);
                var path = bp.Environment.WebRootPath + fileDto.Path;
                if (thumbnail)
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
                ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, null);
                return null;
            }
        }
    }
}