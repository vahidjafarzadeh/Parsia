using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.Context;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Parsia.Core.ComboVal;
using Parsia.Core.Elastic;
using Parsia.Core.File;
using Parsia.Core.Location;

namespace Parsia.Core.Organization
{
    [ClassDetails(Clazz = "Organization", Facade = "OrganizationFacade")]
    public class OrganizationFacade
    {
        private static readonly OrganizationFacade Facade = new OrganizationFacade();
        private static readonly OrganizationCopier Copier = new OrganizationCopier();
        private static readonly ClassDetails[] ClassDetails = (ClassDetails[])typeof(OrganizationFacade).GetCustomAttributes(typeof(ClassDetails), true);

        private OrganizationFacade()
        {
        }
        public static OrganizationFacade GetInstance()
        {
            return Facade;
        }

        public string GetOrgAccess(long orgId)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    return unitOfWork.Organization.Get(p => p.EntityId == orgId).Select(p => p.AccessKey).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade+methodName, null);
                return "";
            }
        }
        public ServiceResult<object> GridView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Organization.Organization>();
                var queryString = $"select * from ( select EntityId as entityId,Name as name,child.parentName,ParentId as parentId,FullTitle as fullTitle,Deleted as deleted , AccessKey as accessKey , CreateBy as createBy from {tableName} as org left join (select EntityId as parentEntityId,Name as parentName from {tableName}) child on child.parentEntityId = org.ParentId ) e " +
                                 QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, false)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var orgList = unitOfWork.Organization.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString()
                    });
                    if (orgList.Count == 0)
                        return new ServiceResult<object>(new List<OrganizationDto>(), 1);
                    var list = new List<object>();
                    var headerTitle = new object[] { "entityId", "name", "parent" };
                    list.Add(headerTitle);
                    list.AddRange(orgList);
                    return new ServiceResult<object>(list, orgList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, OrganizationDto dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                DataLayer.Model.Core.Organization.Organization organization;
                if (dto.EntityId == 0)
                    using (var unitOfWork = new UnitOfWork())
                    {
                        organization = Copier.GetEntity(dto, bp, true);
                        unitOfWork.Organization.Insert(organization);
                        unitOfWork.Organization.Save();
                    }
                else
                    using (var unitOfWork = new UnitOfWork())
                    {
                        organization = Copier.GetEntity(dto, bp, false);
                        unitOfWork.Organization.Update(organization);
                        unitOfWork.Organization.Save();
                    }
                Elastic<OrganizationDto, DataLayer.Model.Core.Organization.Organization>.SaveToElastic(organization, ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(Copier.GetDto(organization), 1);
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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", ClassDetails[0].Facade + methodName,
                        bp.UserInfo);
                using (var context = new ParsiContext())
                {
                    var data = context.Organization.Where(p => p.EntityId == entityId)
                        .Include(p => p.CurrentLogo)
                        .Include(p => p.CurrentOrganizationGrade)
                        .Include(p => p.CurrentOrganizationOwnershipType)
                        .Include(p => p.CurrentOrganizationRoadType)
                        .Include(p => p.CurrentOrganizationStatus)
                        .Include(p => p.CurrentOrganizationType)
                        .Include(p => p.CurrentProvince)
                        .Include(p => p.CurrentCity)
                        .Include(p => p.CurrentOrganization)
                        .Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity)
                        .IgnoreQueryFilters()
                        .ToList();
                    return data.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکورد یافت نشد")
                        : new ServiceResult<object>(Copier.GetDto(data[0]), 1);

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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", ClassDetails[0].Facade + methodName,
                        bp.UserInfo);
                DataLayer.Model.Core.Organization.Organization organization;
                using (var unitOfWork = new UnitOfWork())
                {
                    organization = unitOfWork.Organization.GetRecord(entityId);
                }

                if (organization == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", ClassDetails[0].Facade + methodName,
                        bp.UserInfo);

                organization.Deleted = organization.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Organization.Update(organization);
                    unitOfWork.Organization.Save();
                }
                Elastic<OrganizationDto, DataLayer.Model.Core.Organization.Organization>.SaveToElastic(organization, ClassDetails[0].Clazz, bp);
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
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Organization.Organization>();
                var queryString = "select * from (select EntityId as entityId,Name as name,FullTitle as fullTitle,CreateBy as createBy,AccessKey as accessKey,Deleted as deleted from " +
                                  tableName + " ) e" +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, false)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var orgList = unitOfWork.Organization.CreateNativeQuery(queryString, x => new OrganizationDto()
                    {
                        EntityId = Convert.ToInt64(x[0].ToString()),
                        Name = x[1]?.ToString(),
                        FullTitle = x[2]?.ToString(),
                        Code = x[3]?.ToString()
                    });
                    return orgList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(orgList, orgList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }


        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new OrganizationDto();
            if (!string.IsNullOrEmpty(request.Form["EntityId"])) dto.EntityId = Convert.ToInt64(request.Form["EntityId"]);
            if (!string.IsNullOrEmpty(request.Form["name"])) dto.Name = request.Form["Name"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام سازمان را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["establishingYear"])) dto.EstablishingYear = Convert.ToDouble(request.Form["establishingYear"]);
            if (!string.IsNullOrEmpty(request.Form["order"])) dto.Order = request.Form["order"];
            if (!string.IsNullOrEmpty(request.Form["longitude"])) dto.Longitude = request.Form["longitude"];
            if (!string.IsNullOrEmpty(request.Form["latitude"])) dto.Latitude = request.Form["latitude"];
            if (!string.IsNullOrEmpty(request.Form["area"])) dto.Area = request.Form["area"];
            if (!string.IsNullOrEmpty(request.Form["numberOfFloors"])) dto.NumberOfFloors = request.Form["numberOfFloors"];
            if (!string.IsNullOrEmpty(request.Form["numberOfRooms"])) dto.NumberOfRooms = request.Form["numberOfRooms"];
            if (!string.IsNullOrEmpty(request.Form["yearOfConstruction"])) dto.YearOfConstruction = Convert.ToDouble(request.Form["yearOfConstruction"]);
            if (!string.IsNullOrEmpty(request.Form["fax"])) dto.Fax = request.Form["fax"];
            if (!string.IsNullOrEmpty(request.Form["mobile"])) dto.Mobile = request.Form["mobile"];
            if (!string.IsNullOrEmpty(request.Form["email"])) dto.Email = request.Form["email"];
            if (!string.IsNullOrEmpty(request.Form["address"])) dto.Address = request.Form["address"];
            if (!string.IsNullOrEmpty(request.Form["smsNumber"])) dto.SmsNumber = request.Form["smsNumber"];
            if (!string.IsNullOrEmpty(request.Form["telPhone"])) dto.TelPhone = request.Form["telPhone"];
            if (!string.IsNullOrEmpty(request.Form["code"])) dto.Code = request.Form["code"];
            if (!string.IsNullOrEmpty(request.Form["description"])) dto.Description = request.Form["description"];
            if (!string.IsNullOrEmpty(request.Form["active"])) dto.Active = Convert.ToBoolean(request.Form["active"]);
            if (!string.IsNullOrEmpty(request.Form["parent"])) dto.Parent = new OrganizationDto(){EntityId = Convert.ToInt64(request.Form["parent"]) };
            if (!string.IsNullOrEmpty(request.Form["organizationStatus"])) dto.OrganizationStatus =new ComboValDto(){EntityId = Convert.ToInt64(request.Form["organizationStatus"]) };
            if (!string.IsNullOrEmpty(request.Form["organizationOwnershipType"])) dto.OrganizationOwnershipType = new ComboValDto() { EntityId = Convert.ToInt64(request.Form["organizationOwnershipType"]) };
            if (!string.IsNullOrEmpty(request.Form["organizationRoadType"])) dto.OrganizationRoadType = new ComboValDto() { EntityId = Convert.ToInt64(request.Form["organizationRoadType"]) };
            if (!string.IsNullOrEmpty(request.Form["province"])) dto.Province = new LocationDto() { EntityId = Convert.ToInt64(request.Form["province"]) };
            if (!string.IsNullOrEmpty(request.Form["city"])) dto.City = new LocationDto() { EntityId = Convert.ToInt64(request.Form["city"]) };
            if (!string.IsNullOrEmpty(request.Form["organizationType"])) dto.OrganizationType = new ComboValDto() { EntityId = Convert.ToInt64(request.Form["organizationType"]) };
            if (!string.IsNullOrEmpty(request.Form["organizationGrade"])) dto.OrganizationGrade = new ComboValDto() { EntityId = Convert.ToInt64(request.Form["organizationGrade"]) };
            if (!string.IsNullOrEmpty(request.Form["Logo"])) dto.Logo = new FileDto() { EntityId = Convert.ToInt64(request.Form["Logo"]) };
            if (!string.IsNullOrEmpty(request.Form["aboutUs"])) dto.AboutUs = request.Form["aboutUs"];
            if (!string.IsNullOrEmpty(request.Form["Ticket"])) dto.Ticket = request.Form["Ticket"];
            return new ServiceResult<object>(dto, 1);
        }

        public string SetAccessKey(OrganizationDto organizationDto, long? parentId, BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Organization.Organization>();
                using (var unitOfWork = new UnitOfWork())
                {
                    var parentOrganization = unitOfWork.Organization.Get(p => p.EntityId == parentId)
                        .FirstOrDefault();
                    var queryString = parentId == null
                        ? $"SELECT max(AccessKey) FROM {tableName} WHERE ParentId is null AND Deleted=0"
                        : $"SELECT max(AccessKey) FROM {tableName} WHERE ParentId = {parentId} AND Deleted=0";
                    var data = unitOfWork.ComboVal.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0],
                    });
                    if (data.Count > 0)
                    {
                        var ak = data[0][0].ToString();
                        var siblingLastPart = "";
                        if (ak != null)
                        {
                            siblingLastPart = ak.Substring(ak.Length - 3);
                        }
                        else
                        {
                            var query = parentId == null
                                ? $"SELECT AccessKey FROM {tableName} WHERE entityId is null AND Deleted=0"
                                : $"SELECT AccessKey FROM {tableName} WHERE entityId = {parentId} AND Deleted=0";
                            var orgIds = unitOfWork.ComboVal.CreateNativeQuery(query, x => new[]
                            {
                                x[0],
                            });
                            if (orgIds.Count > 0)
                            {
                                siblingLastPart = orgIds[0].ToString();
                            }
                        }
                        var nextKey = NextKey(siblingLastPart);
                        if (parentOrganization?.AccessKey != null)
                            return parentOrganization.AccessKey + nextKey;
                        else
                            return nextKey;


                    }
                    else
                    {
                        if (parentOrganization?.AccessKey != null)
                            return parentOrganization.AccessKey + "AAA";
                        else
                            return "AAA";
                    }

                }
            }
            catch (Exception e)
            {
                ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
                return "AAA";
            }
        }
        public string NextKey(string key)
        {
            if (key.Equals("ZZZ"))
                throw new Exception("Organization access key last part exceed from (ZZZ). Groups organizations to smaller counts");
            var next = Util.Base26Code((Util.Base26Number(key) + 1));
            return "" + next;
        }
    }
}