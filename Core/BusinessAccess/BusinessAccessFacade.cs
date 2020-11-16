using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLayer.Base;
using DataLayer.Context;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Parsia.Core.Elastic;
using Parsia.Core.Organization;
using Parsia.Core.Role;

namespace Parsia.Core.BusinessAccess
{
    public class BusinessAccessFacade : IBaseFacade<BusinessAccessDto>
    {
        private static readonly BusinessAccessFacade Facade = new BusinessAccessFacade();
        private static readonly BusinessAccessCopier Copier = new BusinessAccessCopier();

        private BusinessAccessFacade()
        {
        }

        public BusinessAccessDto AddList(BusinessParam bp, BusinessAccessDto dto)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var item = unitOfWork.BusinessAccess.Get(p => p.UseCase == dto.UseCase && p.Role == dto.Role.EntityId && p.Organization == dto.Organization.EntityId).FirstOrDefault();
                    if (item != null)
                    {

                        var lstIds = new HashSet<string>();
                        var split = dto.EntityIds.Split(","); 
                        foreach (var s in split)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                lstIds.Add(s.Trim());
                            }
                        }
                        if (dto.AddCurrentList)

                        {
                            var split2 = item.EntityIds.Split(",");
                            foreach (var s in split2)
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    lstIds.Add(s.Trim());
                                }
                            }
                        }
                        var result = new StringBuilder();
                        foreach (var data in lstIds)
                        {
                            result.Append(data).Append(",");
                        }
                        item.EntityIds = result.ToString().Substring(0, result.ToString().Length - 1);
                        return Copier.GetDto(item);
                    }

                    return dto;
                }
            }
            catch (Exception e)
            {
                ExceptionUtil.ExceptionHandler(e, "BusinessAccessFacade.AddList", bp.UserInfo);
                return dto;
            }

        }
        public ServiceResult<object> GridView(BusinessParam bp)
        {
            try
            {
                var tblBusiness = Util.GetSqlServerTableName<DataLayer.Model.Core.BusinessAccess.BusinessAccess>();
                var tblRole = Util.GetSqlServerTableName<DataLayer.Model.Core.Role.Role>();
                var tblOrganization = Util.GetSqlServerTableName<DataLayer.Model.Core.Organization.Organization>();
                var queryString = $"select entityId,organizationName,roleRoleName,useCase,fullTitle,deleted,createBy,accessKey from ( select EntityId as entityId,UseCase as useCase,mainRole.roleName as roleRoleName , mainOrg.orgName as organizationName, FullTitle as fullTitle,CreateBy as createBy,Deleted as deleted,AccessKey as accessKey from {tblBusiness} as mainData left join (select EntityId as roleEntityId,RoleName as roleName from {tblRole}) as mainRole on mainRole.roleEntityId = mainData.Role left join (select EntityId as orgEntityId , Name as orgName from {tblOrganization}) as mainOrg on mainOrg.orgEntityId = mainData.Organization ) e" +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, "Location", false, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var locationList = unitOfWork.Person.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString(),
                        x[4]?.ToString(),
                    });
                    if (locationList.Count == 0)
                        return new ServiceResult<object>(new List<RoleDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[] { "entityId", "organizationName", "roleRoleName", "usecase" };
                    list.Add(headerTitle);
                    list.AddRange(locationList);
                    return new ServiceResult<object>(list, locationList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "BusinessAccessFacade.GridView", bp.UserInfo);
            }
        }
        public ServiceResult<object> Save(BusinessParam bp, BusinessAccessDto dto)
        {
            try
            {
                DataLayer.Model.Core.BusinessAccess.BusinessAccess businessAccess;

                dto = AddList(bp, dto);

                if (dto.EntityId == 0)

                    using (var unitOfWork = new UnitOfWork())
                    {
                        businessAccess = Copier.GetEntity(dto, bp, true);
                        unitOfWork.BusinessAccess.Insert(businessAccess);
                        unitOfWork.BusinessAccess.Save();
                    }
                else
                    using (var unitOfWork = new UnitOfWork())
                    {
                        businessAccess = Copier.GetEntity(dto, bp, false);
                        unitOfWork.BusinessAccess.Update(businessAccess);
                        unitOfWork.BusinessAccess.Save();
                    }

                Elastic<BusinessAccessDto, DataLayer.Model.Core.BusinessAccess.BusinessAccess>.SaveToElastic(businessAccess, "BusinessAccess", bp);
                return new ServiceResult<object>(Copier.GetDto(businessAccess), 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "BusinessAccessFacade.Save", bp.UserInfo);
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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "BusinessAccessFacade.ShowRow",
                        bp.UserInfo);
                using (var context = new ParsiContext())
                {
                    var businessAccesses = context.BusinessAccesses.Where(p => p.EntityId == entityId)
                        .Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity)
                        .Include(p => p.CurrentRole)
                        .Include(p => p.CurrentOrganization)
                        .ToList();
                    return new ServiceResult<object>(Copier.GetDto(businessAccesses[0]), 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "BusinessAccessFacade.ShowRow", bp.UserInfo);
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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "BusinessAccessFacade.Delete",
                        bp.UserInfo);
                DataLayer.Model.Core.BusinessAccess.BusinessAccess businessAccess;
                using (var unitOfWork = new UnitOfWork())
                {
                    businessAccess = unitOfWork.BusinessAccess.GetRecord(entityId);
                }

                if (businessAccess == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "BusinessAccessFacade.Delete",
                        bp.UserInfo);
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.BusinessAccess.Delete(businessAccess);
                    unitOfWork.BusinessAccess.Save();
                }
                Elastic<BusinessAccessDto, DataLayer.Model.Core.BusinessAccess.BusinessAccess>.SaveToElastic(businessAccess, "BusinessAccess", bp);
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "BusinessAccessFacade.Delete", bp.UserInfo);
            }
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.BusinessAccess.BusinessAccess>();
                var queryString = $"select * from (select EntityId as entityId,FullTitle as fullTitle,Deleted as deleted,AccessKey as accessKey,CreateBy as createBy from {tableName}) e " +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, "BusinessAccess", true, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var locationList = unitOfWork.Location.CreateNativeQuery(queryString, x => new Dictionary<string, object>()
                    {
                        {"entityId",Convert.ToInt64(x[0].ToString()) },
                        {"fullTitle",x[1]?.ToString() }
                    });
                    return locationList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(locationList, locationList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, "BusinessAccessFacade.AutocompleteView", bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new BusinessAccessDto();
            if (!string.IsNullOrEmpty(request.Form["entityId"])) dto.EntityId = Convert.ToInt64(request.Form["entityId"]);
            if (!string.IsNullOrEmpty(request.Form["organization"])) dto.Organization = new OrganizationDto() { EntityId = Convert.ToInt64(request.Form["organization"]) }; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا سازمان را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["role"])) dto.Role = new RoleDto() { EntityId = Convert.ToInt64(request.Form["role"]) }; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نقش را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["usecase"])) dto.UseCase = request.Form["usecase"];
            if (!string.IsNullOrEmpty(request.Form["entityIds"])) dto.EntityIds = request.Form["entityIds"];
            if (!string.IsNullOrEmpty(request.Form["addCurrentList"])) dto.AddCurrentList = Convert.ToBoolean(request.Form["addCurrentList"]);
            if (!string.IsNullOrEmpty(request.Form["Active"])) dto.Active = Convert.ToBoolean(request.Form["Active"]);
            if (!string.IsNullOrEmpty(request.Form["Ticket"])) dto.Ticket = request.Form["Ticket"];
            if (!string.IsNullOrEmpty(request.Form["code"])) dto.Code = request.Form["code"];
            return new ServiceResult<object>(dto, 1);
        }

        public static BusinessAccessFacade GetInstance()
        {
            return Facade;
        }
    }
}