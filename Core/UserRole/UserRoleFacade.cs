using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Base;
using DataLayer.Context;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Parsia.Core.Elastic;
using Parsia.Core.Organization;
using Parsia.Core.Role;

namespace Parsia.Core.UserRole
{
    public class UserRoleFacade : IBaseFacade<UserRoleDto>
    {
        private static readonly UserRoleFacade Facade = new UserRoleFacade();
        private static readonly UserRoleCopier Copier = new UserRoleCopier();
        private UserRoleFacade()
        {
        }
        public ServiceResult<object> GridView(BusinessParam bp)
        {
            try
            {
                var tblUserRole = Util.GetSqlServerTableName<DataLayer.Model.Core.UserRole.UserRole>();
                var tblUser = Util.GetSqlServerTableName<DataLayer.Model.Core.User.Users>();
                var tblRole = Util.GetSqlServerTableName<DataLayer.Model.Core.Role.Role>();
                var tblOrganization = Util.GetSqlServerTableName<DataLayer.Model.Core.Organization.Organization>();
                var queryString = $"select entityId,userName,role,organization,deleted,fullTitle,accessKey,createBy from ( select EntityId as entityId,currentRole.roleName as role,currentOrganization.orgName as organization,currentUser.userName as userName,Deleted as deleted,AccessKey as accessKey,CreateBy as createBy,FullTitle as fullTitle from {tblUserRole} mainData left join (select EntityId as roleEntityId,RoleName as roleName from {tblRole}) as currentRole on currentRole.roleEntityId = mainData.RoleId left join (select EntityId as orgId, Name as orgName from {tblOrganization}) as currentOrganization on currentOrganization.orgId = mainData.OrganizationId left join (select EntityId as userId,Username as userName from {tblUser}) as currentUser on currentUser.userId = mainData.UserId ) e " +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, "UserRole", false, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboList = unitOfWork.UserRole.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString(),
                        x[4]?.ToString()
                    });
                    if (comboList.Count == 0)
                        return new ServiceResult<object>(new List<UserRoleDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[] { "entityId", "userName", "role","organization" };
                    list.Add(headerTitle);
                    list.AddRange(comboList);
                    return new ServiceResult<object>(list, comboList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UserRoleFacade.GridView", bp.UserInfo);
            }
        }
        public ServiceResult<object> Save(BusinessParam bp, UserRoleDto dto)
        {
            try
            {
                DataLayer.Model.Core.UserRole.UserRole userRole;
                if (dto.EntityId == 0)
                {
                    using (var unitOfWork = new UnitOfWork())
                    {
                        userRole = Copier.GetEntity(dto, bp, true);
                        unitOfWork.UserRole.Insert(userRole);
                        unitOfWork.UserRole.Save();
                    }
                }

                else
                {

                    using (var unitOfWork = new UnitOfWork())
                    {
                        userRole = Copier.GetEntity(dto, bp, false);
                        unitOfWork.UserRole.Update(userRole);
                        unitOfWork.UserRole.Save();
                    }
                }

                Elastic<UserRoleDto, DataLayer.Model.Core.UserRole.UserRole>.SaveToElastic(userRole, "UserRoleFacade", bp);
                return new ServiceResult<object>(Copier.GetDto(userRole), 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UserRoleFacade.Save", bp.UserInfo);
            }
        }
        public ServiceResult<object> ShowRow(BusinessParam bp)
        {
            long entityId = 0;
            foreach (var where in bp.Clause.Wheres.Where(where =>
                where.Key.Equals("entityId") && where.Value != null && !where.Value.Equals("")))
                entityId = long.Parse(where.Value);

            try
            {
                if (entityId == 0)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "UserRoleFacade.ShowRow",
                        bp.UserInfo);
                using (var context = new ParsiContext())
                {
                    var userRole = context.UserRole.Where(p => p.EntityId == entityId)
                        .Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity)
                        .Include(p => p.CurrentRole)
                        .Include(p => p.CurrentOrganization)
                        .Include(p => p.CurrentUsers)
                        .ToList();
                    return new ServiceResult<object>(Copier.GetDto(userRole[0]), 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UserRoleFacade.ShowRow", bp.UserInfo);
            }
        }

        public ServiceResult<object> Delete(BusinessParam bp)
        {
            long entityId = 0;
            foreach (var where in bp.Clause.Wheres.Where(where =>
                where.Key.Equals("entityId") && where.Value != null && !where.Value.Equals("")))
                entityId = long.Parse(where.Value);

            try
            {
                if (entityId == 0)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "UserRoleFacade.Delete",
                        bp.UserInfo);
                DataLayer.Model.Core.UserRole.UserRole userRole;
                using (var unitOfWork = new UnitOfWork())
                {
                    userRole = unitOfWork.UserRole.GetRecord(entityId);
                }

                if (userRole == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "UserRoleFacade.Delete",
                        bp.UserInfo);

                userRole.Deleted = userRole.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.UserRole.Update(userRole);
                    unitOfWork.UserRole.Save();
                }
                Elastic<UserRoleDto, DataLayer.Model.Core.UserRole.UserRole>.SaveToElastic(userRole, "UserRole", bp);
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UserRoleFacade.Delete", bp.UserInfo);
            }
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.UserRole.UserRole>();
                var queryString = $"select * from (select EntityId as entityId,FullTitle as fullTitle,Deleted as deleted,AccessKey as accessKey,CreateBy as createBy from {tableName}) e " +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, "UserRole", true, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboList = unitOfWork.Person.CreateNativeQuery(queryString, x => new Dictionary<string, object>()
                    {
                        {"entityId",Convert.ToInt64(x[0].ToString()) },
                        {"fullTitle",x[1]?.ToString() }
                    });
                    return comboList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(comboList, comboList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, "UserRoleFacade.AutocompleteView", bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new UserRoleDto();
            if (!string.IsNullOrEmpty(request.Form["entityId"])) dto.EntityId = Convert.ToInt64(request.Form["entityId"]);
            if (!string.IsNullOrEmpty(request.Form["user"])) dto.User = new UserDto(){EntityId = Convert.ToInt64(request.Form["user"])}; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام کاربر را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["role"])) dto.Role = new RoleDto(){ EntityId = Convert.ToInt64(request.Form["role"]) }; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نقش را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["organization"])) dto.Organization = new OrganizationDto(){ EntityId = Convert.ToInt64(request.Form["organization"]) }; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا سازمان را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["Active"])) dto.Active = Convert.ToBoolean(request.Form["Active"]);
            if (!string.IsNullOrEmpty(request.Form["Ticket"])) dto.Ticket = request.Form["Ticket"];
            if (!string.IsNullOrEmpty(request.Form["code"])) dto.Code = request.Form["code"];
            dto.OrgAccess = OrganizationFacade.GetInstance().GetOrgAccess(dto.Organization.EntityId);
            return new ServiceResult<object>(dto, 1);
        }

        public static UserRoleFacade GetInstance()
        {
            return Facade;
        }
    }
}