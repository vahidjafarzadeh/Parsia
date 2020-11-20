using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.Base;
using DataLayer.Context;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Parsia.Core.AccessGroup;
using Parsia.Core.Elastic;
using Parsia.Core.roleAccessGroup;
using Parsia.Core.RoleAccessGroup;

namespace Parsia.Core.Role
{
    [ClassDetails(Clazz = "Role", Facade = "RoleFacade")]
    public class RoleFacade : IBaseFacade<RoleDto>
    {
        private static readonly RoleFacade Facade = new RoleFacade();
        private static readonly RoleCopier Copier = new RoleCopier();

        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(RoleFacade).GetCustomAttributes(typeof(ClassDetails), true);

        private RoleFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Role.Role>();
                var queryString =
                    $"select entityId,roleName,expireMinute,deleted,fullTitle,createBy,accessKey from (select EntityId as entityId, RoleName as roleName,ExpireMinute as expireMinute,FullTitle as fullTitle,Deleted as deleted,CreateBy as createBy,AccessKey as accessKey from {tableName} where code <> 'ADMIN') e " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var roleList = unitOfWork.Role.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString()
                    });
                    if (roleList.Count == 0)
                        return new ServiceResult<object>(new List<RoleDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[] {"entityId", "roleName", "expireMinute"};
                    list.Add(headerTitle);
                    list.AddRange(roleList);
                    return new ServiceResult<object>(list, roleList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, RoleDto dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                DataLayer.Model.Core.Role.Role role;
                if (dto.EntityId == 0)
                {
                    using (var unitOfWork = new UnitOfWork())
                    {
                        role = Copier.GetEntity(dto, bp, true);
                        unitOfWork.Role.Insert(role);
                        unitOfWork.Role.Save();
                    }

                    if (dto.AccessGroupDto != null)
                    {
                        var lstGroup = dto.AccessGroupDto.Select(item => new RoleAccessGroupDto
                            {Role = Copier.GetDto(role), AccessGroup = item}).ToList();
                        var serviceResult = RoleAccessGroupFacade.GetInstance().SaveList(bp, lstGroup);
                        if (!serviceResult.Done) return serviceResult;
                    }
                }

                else
                {
                    var deletedList = RoleAccessGroupFacade.GetInstance().DeletedList(bp, dto.EntityId);
                    if (deletedList.Done)
                    {
                        using (var unitOfWork = new UnitOfWork())
                        {
                            role = Copier.GetEntity(dto, bp, false);
                            unitOfWork.Role.Update(role);
                            unitOfWork.Role.Save();
                        }

                        if (dto.AccessGroupDto != null)
                        {
                            var lstGroup = dto.AccessGroupDto.Select(item => new RoleAccessGroupDto
                                {Role = Copier.GetDto(role), AccessGroup = item}).ToList();
                            var serviceResult = RoleAccessGroupFacade.GetInstance().SaveList(bp, lstGroup);
                            if (!serviceResult.Done) return serviceResult;
                        }
                    }
                    else
                    {
                        return deletedList;
                    }
                }

                Elastic<RoleDto, DataLayer.Model.Core.Role.Role>.SaveToElastic(role, ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(Copier.GetDto(role), 1);
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
                using (var context = new ParsiContext())
                {
                    var role = context.Role.Where(p => p.EntityId == entityId)
                        .Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity)
                        .Include(p => p.ParentRoleAccessGroup)
                        .ThenInclude(p => p.CurrentAccessGroup)
                        .IgnoreQueryFilters()
                        .ToList();
                    return new ServiceResult<object>(Copier.GetDto(role[0]), 1);
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
                DataLayer.Model.Core.Role.Role role;
                using (var unitOfWork = new UnitOfWork())
                {
                    role = unitOfWork.Role.GetRecord(entityId);
                }

                if (role == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد",
                        ClassDetails[0].Facade + methodName,
                        bp.UserInfo);

                role.Deleted = role.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Role.Update(role);
                    unitOfWork.Role.Save();
                }

                Elastic<RoleDto, DataLayer.Model.Core.Role.Role>.SaveToElastic(role, ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(true, 1);
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
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Role.Role>();
                var queryString =
                    $"select * from (select EntityId as entityId,RoleName as roleName,ExpireMinute as expireMinute,FullTitle as fullTitle,Deleted as deleted,AccessKey as accessKey,CreateBy as createBy from {tableName}) e " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, true, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var roleList = unitOfWork.Role.CreateNativeQuery(queryString, x => new Dictionary<string, object>
                    {
                        {"entityId", Convert.ToInt64(x[0].ToString())},
                        {"roleName", x[1]?.ToString()},
                        {"expireMinute", x[2]?.ToString()},
                        {"fullTitle", x[3]?.ToString()}
                    });
                    return roleList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(roleList, roleList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new RoleDto();
            if (!string.IsNullOrEmpty(request.Form["roleName"])) dto.RoleName = request.Form["roleName"];
            else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام نقش را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["expireMinute"]))
                dto.ExpireMinute = Convert.ToInt64(request.Form["expireMinute"]);
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                    "لطفا زمان پایان سشن را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["active"])) dto.Active = Convert.ToBoolean(request.Form["active"]);
            if (!string.IsNullOrEmpty(request.Form["ticket"])) dto.Ticket = request.Form["ticket"];
            if (!string.IsNullOrEmpty(request.Form["code"])) dto.Code = request.Form["code"];
            if (!string.IsNullOrEmpty(request.Form["entityId"]))
                dto.EntityId = Convert.ToInt64(request.Form["entityId"]);
            if (!string.IsNullOrEmpty(request.Form["accessGroup"]))
            {
                var strings = request.Form["accessGroup"].ToString().Split(",");
                var lstData = (from item in strings
                    where !string.IsNullOrEmpty(item)
                    select new AccessGroupDto {EntityId = Convert.ToInt64(item)}).ToList();
                dto.AccessGroupDto = lstData;
            }

            return new ServiceResult<object>(dto, 1);
        }

        public static RoleFacade GetInstance()
        {
            return Facade;
        }
    }
}