using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.Context;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Parsia.Core.Action;
using Parsia.Core.Elastic;
using Parsia.Core.UseCase;
using Parsia.Core.UseCaseAction;
using Parsia.Core.UseCaseActionAccessGroup;

namespace Parsia.Core.AccessGroup
{
    [ClassDetails(Clazz = "AccessGroup", Facade = "AccessGroupFacade")]
    public class AccessGroupFacade
    {
        private static readonly AccessGroupFacade Facade = new AccessGroupFacade();
        private static readonly AccessGroupCopier Copier = new AccessGroupCopier();

        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(AccessGroupFacade).GetCustomAttributes(typeof(ClassDetails), true);

        private AccessGroupFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.AccessGroup.AccessGroup>();
                var queryString =
                    $"select entityId,name,deleted,fullTitle,createBy,accessKey from (select EntityId as entityId,Name as name,Deleted as deleted,FullTitle as fullTitle,CreateBy as createBy,AccessKey as accessKey from {tableName} where code <> 'ADMIN') e  " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var accessGroupList = unitOfWork.AccessGroup.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString()
                    });
                    if (accessGroupList.Count == 0)
                        return new ServiceResult<object>(new List<AccessGroupDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[] {"entityId", "name"};
                    list.Add(headerTitle);
                    list.AddRange(accessGroupList);
                    return new ServiceResult<object>(list, accessGroupList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, List<UseCaseActionAccessGroupDto> dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                DataLayer.Model.Core.AccessGroup.AccessGroup accessGroup;
                if (dto[0].AccessGroup.EntityId == 0)
                {
                    using (var unitOfWork = new UnitOfWork())
                    {
                        accessGroup = Copier.GetEntity(dto[0].AccessGroup, bp, true);
                        unitOfWork.AccessGroup.Insert(accessGroup);
                        unitOfWork.AccessGroup.Save();
                    }

                    foreach (var item in dto) item.AccessGroup = Copier.GetDto(accessGroup);
                    var serviceResult = UseCaseActionAccessGroupFacade.GetInstance().SaveList(bp, dto);
                    if (!serviceResult.Done) return serviceResult;
                }
                else
                {
                    var delete = UseCaseActionAccessGroupFacade.GetInstance()
                        .DeletedList(bp, dto[0].AccessGroup.EntityId);
                    if (delete.Done)
                    {
                        using (var unitOfWork = new UnitOfWork())
                        {
                            accessGroup = Copier.GetEntity(dto[0].AccessGroup, bp, false);
                            unitOfWork.AccessGroup.Update(accessGroup);
                            unitOfWork.AccessGroup.Save();
                        }

                        foreach (var item in dto) item.AccessGroup = Copier.GetDto(accessGroup);
                        var serviceResult = UseCaseActionAccessGroupFacade.GetInstance().SaveList(bp, dto);
                        if (!serviceResult.Done) return serviceResult;
                    }
                    else
                    {
                        return delete;
                    }
                }

                Elastic<AccessGroupDto, DataLayer.Model.Core.AccessGroup.AccessGroup>.SaveToElastic(accessGroup,
                    ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(Copier.GetDto(accessGroup), 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> ShowRow(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            long entityId = 0;
            foreach (var where in bp.Clause.Wheres.Where(where =>
                where.Key.Equals("entityId") && where.Value != null && !where.Value.Equals("")))
                entityId = long.Parse(where.Value);

            try
            {
                if (entityId == 0)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد",
                        ClassDetails[0].Facade + methodName,
                        bp.UserInfo);
                using (var content = new ParsiContext())
                {
                    var data = content.AccessGroup.Where(p => p.EntityId == entityId).Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity).Include(p => p.ParentAccessGroupUseCaseActionAccessGroup)
                        .ThenInclude(p => p.CurrentUseCaseAction).ThenInclude(p => p.CurrentAction).IgnoreQueryFilters()
                        .ToList();
                    return new ServiceResult<object>(Copier.GetDto(data[0]), 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> Delete(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            long entityId = 0;
            foreach (var where in bp.Clause.Wheres.Where(where =>
                where.Key.Equals("entityId") && where.Value != null && !where.Value.Equals("")))
                entityId = long.Parse(where.Value);

            try
            {
                if (entityId == 0)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد",
                        ClassDetails[0].Facade + methodName,
                        bp.UserInfo);
                DataLayer.Model.Core.AccessGroup.AccessGroup accessGroup;
                using (var unitOfWork = new UnitOfWork())
                {
                    accessGroup = unitOfWork.AccessGroup.GetRecord(entityId);
                }

                if (accessGroup == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد",
                        ClassDetails[0].Facade + methodName,
                        bp.UserInfo);

                accessGroup.Deleted = accessGroup.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.AccessGroup.Update(accessGroup);
                    unitOfWork.AccessGroup.Save();
                }

                Elastic<AccessGroupDto, DataLayer.Model.Core.AccessGroup.AccessGroup>.SaveToElastic(accessGroup,
                    ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> GetAllData(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var accessGroups = unitOfWork.AccessGroup.Get(p => p.EntityId != 1).ToList();
                    return new ServiceResult<object>(Copier.GetDto(accessGroups), accessGroups.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.AccessGroup.AccessGroup>();
                var queryString =
                    "select * from (select EntityId,Name,FullTitle,code,CreateBy,AccessKey,Deleted from " +
                    tableName + "  where code <> 'ADMIN') e" +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var accessGroupList = unitOfWork.AccessGroup.CreateNativeQuery(queryString, x =>
                        new Dictionary<string, object>
                        {
                            {"entityId", Convert.ToInt64(x[0].ToString())},
                            {"name", Convert.ToInt64(x[1].ToString())},
                            {"fullTitle", Convert.ToInt64(x[2].ToString())}
                        });
                    return accessGroupList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(accessGroupList, accessGroupList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var lstData = new List<UseCaseActionAccessGroupDto>();
            var accessGroup = new AccessGroupDto();
            if (!string.IsNullOrEmpty(request.Form["EntityId"]))
                accessGroup.EntityId = Convert.ToInt64(request.Form["EntityId"]);
            if (!string.IsNullOrEmpty(request.Form["Name"])) accessGroup.Name = request.Form["Name"];
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                    "لطفا نام گروه دسترسی را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["Code"])) accessGroup.Code = request.Form["Code"];
            if (!string.IsNullOrEmpty(request.Form["Active"]))
                accessGroup.Active = Convert.ToBoolean(request.Form["Active"]);
            if (!string.IsNullOrEmpty(request.Form["Ticket"])) accessGroup.Ticket = request.Form["Ticket"];
            if (!string.IsNullOrEmpty(request.Form["actions"]))
            {
                var strings = request.Form["actions"].ToString().Split(",");
                foreach (var item in strings)
                    if (item.Contains("|"))
                    {
                        var useCaseActionDto = new UseCaseActionDto
                        {
                            Action = new ActionDto {EntityId = Convert.ToInt64(item.Split("|")[1])},
                            UseCase = new UseCaseDto {EntityId = Convert.ToInt64(item.Split("|")[0])}
                        };
                        lstData.Add(new UseCaseActionAccessGroupDto
                            {AccessGroup = accessGroup, UseCaseAction = useCaseActionDto});
                    }
            }
            else
            {
                var dto = new UseCaseActionAccessGroupDto
                {
                    AccessGroup = accessGroup
                };
                lstData.Add(dto);
            }

            return new ServiceResult<object>(lstData, 1);
        }

        public static AccessGroupFacade GetInstance()
        {
            return Facade;
        }
    }
}