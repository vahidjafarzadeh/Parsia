using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.Base;
using DataLayer.Context;
using DataLayer.Model.Core.User;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Parsia.Core.Elastic;
using Parsia.Core.Organization;
using Parsia.Core.Role;

namespace Parsia.Core.EntityState
{
    [ClassDetails(Clazz = "EntityState", Facade = "EntityStateFacade")]
    public class EntityStateFacade : IBaseFacade<EntityStateDto>
    {
        private static readonly EntityStateFacade Facade = new EntityStateFacade();
        private static readonly EntityStateCopier Copier = new EntityStateCopier();

        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(EntityStateFacade).GetCustomAttributes(typeof(ClassDetails), true);

        private EntityStateFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tblUser = Util.GetSqlServerTableName<Users>();
                var tblOrganization = Util.GetSqlServerTableName<DataLayer.Model.Core.Organization.Organization>();
                var tblRole = Util.GetSqlServerTableName<DataLayer.Model.Core.Role.Role>();
                var tblEntity = Util.GetSqlServerTableName<DataLayer.Model.Core.EntityState.EntityState>();
                var queryString =
                    $"select entityId,tableName,tblEntityId,organizationName,userRoleRoleName, userLockerFirstName,userLockerLastName,fullTitle,deleted,createBy,accessKey  from (select EntityId as entityId,FullTitle as fullTitle ,Deleted as deleted,CreateBy as createBy,AccessKey as accessKey,TableName as tableName,TableEntityId as tblEntityId,mainOrganization.orgName as organizationName,mainRole.roleName as userRoleRoleName,mainUser.userFirstName as userLockerFirstName,mainUser.lastName as userLockerLastName  from {tblEntity} as mainData left join (select EntityId as roleEntityId,RoleName as roleName from {tblRole}) as mainRole on mainRole.roleEntityId = mainData.RoleId left join (select EntityId as OrganizationEntityId ,Name as orgName from {tblOrganization}) as mainOrganization on mainOrganization.OrganizationEntityId = mainData.OrganizationId left join (select EntityId as userEntityId,FirstName as userFirstName,LastName as lastName from {tblUser}) as mainUser on mainUser.userEntityId = mainData.UserId ) e  " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var entityStateList = unitOfWork.EntityState.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString(),
                        x[4]?.ToString(),
                        x[5]?.ToString(),
                        x[6]?.ToString(),
                        x[7]?.ToString()
                    });
                    if (entityStateList.Count == 0)
                        return new ServiceResult<object>(new List<EntityStateDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[]
                    {
                        "entityId", "tblName", "tblEntityId", "organizationName", "userRoleRoleName",
                        "userLockerFirstName", "userLockerLastName"
                    };
                    list.Add(headerTitle);
                    list.AddRange(entityStateList);
                    return new ServiceResult<object>(list, entityStateList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, EntityStateDto dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                DataLayer.Model.Core.EntityState.EntityState entityState;
                if (dto.EntityId == 0)

                    using (var unitOfWork = new UnitOfWork())
                    {
                        entityState = Copier.GetEntity(dto, bp, true);
                        unitOfWork.EntityState.Insert(entityState);
                        unitOfWork.EntityState.Save();
                    }
                else
                    using (var unitOfWork = new UnitOfWork())
                    {
                        entityState = Copier.GetEntity(dto, bp, false);
                        unitOfWork.EntityState.Update(entityState);
                        unitOfWork.EntityState.Save();
                    }

                Elastic<EntityStateDto, DataLayer.Model.Core.EntityState.EntityState>.SaveToElastic(entityState,
                    ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(Copier.GetDto(entityState), 1);
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
                    var entityState = context.EntityState.Where(p => p.EntityId == entityId)
                        .Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity)
                        .Include(p => p.CurrentRole)
                        .Include(p => p.CurrentOrganization)
                        .Include(p => p.CurrentUser)
                        .IgnoreQueryFilters()
                        .ToList();
                    return new ServiceResult<object>(Copier.GetDto(entityState[0]), 1);
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
                DataLayer.Model.Core.EntityState.EntityState entityState;
                using (var unitOfWork = new UnitOfWork())
                {
                    entityState = unitOfWork.EntityState.GetRecord(entityId);
                }

                if (entityState == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد",
                        ClassDetails[0].Facade + methodName,
                        bp.UserInfo);
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.EntityState.Delete(entityState);
                    unitOfWork.EntityState.Save();
                }

                Elastic<EntityStateDto, DataLayer.Model.Core.EntityState.EntityState>.SaveToElastic(entityState,
                    ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> Delete(BusinessParam bp, EntityStateDto dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";

            try
            {
                DataLayer.Model.Core.EntityState.EntityState entityState;
                using (var unitOfWork = new UnitOfWork())
                {
                    entityState = unitOfWork.EntityState
                        .Get(p => p.TableEntityId == dto.TableEntityId && p.TableName == dto.TableName)
                        .FirstOrDefault();
                }

                if (entityState == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد",
                        ClassDetails[0].Facade + methodName,
                        bp.UserInfo);
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.EntityState.Delete(entityState);
                    unitOfWork.EntityState.Save();
                }

                Elastic<EntityStateDto, DataLayer.Model.Core.EntityState.EntityState>.SaveToElastic(entityState,
                    ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> GetState(BusinessParam bp, EntityStateDto dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tblUser = Util.GetSqlServerTableName<Users>();
                var tblOrganization = Util.GetSqlServerTableName<DataLayer.Model.Core.Organization.Organization>();
                var tblRole = Util.GetSqlServerTableName<DataLayer.Model.Core.Role.Role>();
                var tblEntity = Util.GetSqlServerTableName<DataLayer.Model.Core.EntityState.EntityState>();
                var queryString =
                    $"select entityId,tableName,tblEntityId,organizationName,userRoleRoleName, userLockerFirstName,userLockerLastName,userEntityId,fullTitle,deleted,createBy,accessKey from(select EntityId as entityId,FullTitle as fullTitle,mainUser.userEntityId as userEntityId ,Deleted as deleted,CreateBy as createBy,AccessKey as accessKey,TableName as tableName,TableEntityId as tblEntityId,mainOrganization.orgName as organizationName,mainRole.roleName as userRoleRoleName,mainUser.userFirstName as userLockerFirstName,mainUser.lastName as userLockerLastName from(select * from {tblEntity} where TableName = N'{dto.TableName}' And  TableEntityId = {dto.TableEntityId}) as mainData left join(select EntityId as roleEntityId,RoleName as roleName from {tblRole}) as mainRole on mainRole.roleEntityId = mainData.RoleId left join(select EntityId as OrganizationEntityId ,Name as orgName from {tblOrganization}) as mainOrganization on mainOrganization.OrganizationEntityId = mainData.OrganizationId left join(select EntityId as userEntityId,FirstName as userFirstName,LastName as lastName from {tblUser}) as mainUser on mainUser.userEntityId = mainData.UserId ) e  " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var entityStateList = unitOfWork.EntityState.CreateNativeQuery(queryString, x =>
                        new Dictionary<string, object>
                        {
                            {"entityId", Convert.ToInt64(x[0].ToString())},
                            {"roleName", x[4].ToString()},
                            {"name", x[5].ToString()},
                            {"family", x[6].ToString()},
                            {"orgName", x[3].ToString()},
                            {"active", bp.UserInfo.UserId == Convert.ToInt64(x[7].ToString()) ? "1" : "0"}
                        });
                    return entityStateList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(entityStateList[0], entityStateList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.EntityState.EntityState>();
                var queryString =
                    $"select * from (select EntityId as entityId,FullTitle as fullTitle,Deleted as deleted,AccessKey as accessKey,CreateBy as createBy from {tableName}) e " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, "Location", true, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var entityStateList = unitOfWork.EntityState.CreateNativeQuery(queryString, x =>
                        new Dictionary<string, object>
                        {
                            {"entityId", Convert.ToInt64(x[0].ToString())},
                            {"fullTitle", x[1]?.ToString()}
                        });
                    return entityStateList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(entityStateList, entityStateList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new EntityStateDto();
            if (!string.IsNullOrEmpty(request.Form["entityId"]))
                dto.EntityId = Convert.ToInt64(request.Form["entityId"]);
            if (!string.IsNullOrEmpty(request.Form["tblName"])) dto.TableName = request.Form["tblName"];
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام جدول را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["tblEntityId"]))
                dto.TableEntityId = Convert.ToInt64(request.Form["tblEntityId"]);
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                    "لطفا شناسه رکورد داخل جدول را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["organization"]))
                dto.Organization = new OrganizationDto {EntityId = Convert.ToInt64(request.Form["organization"])};
            else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا سازمان را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["userRole"]))
                dto.Role = new RoleDto {EntityId = Convert.ToInt64(request.Form["userRole"])};
            else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نقش را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["userLocker"]))
                dto.User = new UserDto {EntityId = Convert.ToInt64(request.Form["userLocker"])};
            else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کاربر را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["active"])) dto.Active = Convert.ToBoolean(request.Form["active"]);
            if (!string.IsNullOrEmpty(request.Form["ticket"])) dto.Ticket = request.Form["ticket"];
            if (!string.IsNullOrEmpty(request.Form["code"])) dto.Code = request.Form["code"];
            return new ServiceResult<object>(dto, 1);
        }

        public ServiceResult<object> GetDtoFromRequestWithCurrentUser(HttpRequest request, BusinessParam bp)
        {
            var dto = new EntityStateDto();
            if (!string.IsNullOrEmpty(request.Form["tblName"])) dto.TableName = request.Form["tblName"];
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام جدول را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["tblEntityId"]))
                dto.TableEntityId = Convert.ToInt64(request.Form["tblEntityId"]);
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                    "لطفا شناسه رکورد داخل جدول را وارد نمایید");
            dto.Organization = new OrganizationDto {EntityId = bp.UserInfo.OrganizationId};
            dto.User = new UserDto {EntityId = bp.UserInfo.UserId};
            dto.Role = new RoleDto {EntityId = bp.UserInfo.RoleId};
            return new ServiceResult<object>(dto, 1);
        }

        public static EntityStateFacade GetInstance()
        {
            return Facade;
        }
    }
}