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
using Parsia.Core.ComboVal;
using Parsia.Core.Elastic;

namespace Parsia.Core.Location
{
    [ClassDetails(Clazz = "Location", Facade = "LocationFacade")]
    public class LocationFacade : IBaseFacade<LocationDto>
    {
        private static readonly LocationFacade Facade = new LocationFacade();
        private static readonly LocationCopier Copier = new LocationCopier();

        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(LocationFacade).GetCustomAttributes(typeof(ClassDetails), true);


        private LocationFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Location.Location>();
                var tableNameComboVal = Util.GetSqlServerTableName<DataLayer.Model.Core.ComboVal.ComboVal>();
                var queryString =
                    $"select entityId,name,typeName,type,parentName,parentId,fullTitle,deleted,createBy,accessKey from ( select combo.comboName as typeName,parent.parentName as parentName, EntityId as entityId, Name as name,Type as type,ParentId as parentId,FullTitle as fullTitle,Deleted as deleted,CreateBy as createBy,AccessKey as accessKey from {tableName} as mainLocation left join (select EntityId as comboId,Name as comboName from {tableNameComboVal}) as combo on combo.comboId = mainLocation.type left join (select EntityId as parentEntityId,Name as parentName from {tableName}) as parent on parent.parentEntityId = mainLocation.ParentId ) e " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var locationList = unitOfWork.Location.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString(),
                        x[5]?.ToString()
                    });
                    if (locationList.Count == 0)
                        return new ServiceResult<object>(new List<LocationDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[] {"entityId", "name", "type", "parent"};
                    list.Add(headerTitle);
                    list.AddRange(locationList);
                    return new ServiceResult<object>(list, locationList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, LocationDto dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                DataLayer.Model.Core.Location.Location location;
                if (dto.EntityId == 0)

                    using (var unitOfWork = new UnitOfWork())
                    {
                        location = Copier.GetEntity(dto, bp, true);
                        unitOfWork.Location.Insert(location);
                        unitOfWork.Location.Save();
                    }
                else
                    using (var unitOfWork = new UnitOfWork())
                    {
                        location = Copier.GetEntity(dto, bp, false);
                        unitOfWork.Location.Update(location);
                        unitOfWork.Location.Save();
                    }

                Elastic<LocationDto, DataLayer.Model.Core.Location.Location>.SaveToElastic(location,
                    ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(Copier.GetDto(location), 1);
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
                    var location = context.Location.Where(p => p.EntityId == entityId)
                        .Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity)
                        .Include(p => p.CurrentType)
                        .Include(p => p.CurrentLocation)
                        .IgnoreQueryFilters()
                        .ToList();
                    return new ServiceResult<object>(Copier.GetDto(location[0]), 1);
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
                DataLayer.Model.Core.Location.Location location;
                using (var unitOfWork = new UnitOfWork())
                {
                    location = unitOfWork.Location.GetRecord(entityId);
                }

                if (location == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد",
                        ClassDetails[0].Facade + methodName,
                        bp.UserInfo);

                location.Deleted = location.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Location.Update(location);
                    unitOfWork.Location.Save();
                }

                Elastic<LocationDto, DataLayer.Model.Core.Location.Location>.SaveToElastic(location,
                    ClassDetails[0].Clazz, bp);
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
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Location.Location>();
                var queryString =
                    $"select * from (select EntityId as entityId,FullTitle as fullTitle,Deleted as deleted,AccessKey as accessKey,CreateBy as createBy from {tableName}) e " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, true, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var locationList = unitOfWork.Location.CreateNativeQuery(queryString, x =>
                        new Dictionary<string, object>
                        {
                            {"entityId", Convert.ToInt64(x[0].ToString())},
                            {"fullTitle", x[1]?.ToString()}
                        });
                    return locationList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(locationList, locationList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new LocationDto();
            if (!string.IsNullOrEmpty(request.Form["entityId"]))
                dto.EntityId = Convert.ToInt64(request.Form["entityId"]);
            if (!string.IsNullOrEmpty(request.Form["type"]))
                dto.Type = new ComboValDto {EntityId = Convert.ToInt64(request.Form["type"])};
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نوع مکان را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["name"])) dto.Name = request.Form["name"];
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام مکان را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["parent"]))
                dto.Parent = new LocationDto {EntityId = Convert.ToInt64(request.Form["parent"])};
            if (!string.IsNullOrEmpty(request.Form["Active"])) dto.Active = Convert.ToBoolean(request.Form["Active"]);
            if (!string.IsNullOrEmpty(request.Form["Ticket"])) dto.Ticket = request.Form["Ticket"];
            if (!string.IsNullOrEmpty(request.Form["code"])) dto.Code = request.Form["code"];
            return new ServiceResult<object>(dto, 1);
        }

        public string GetParent(BusinessParam bp, long? entityId)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                if (entityId == null) return "";
                using (var unitOfWork = new UnitOfWork())
                {
                    return unitOfWork.Location.Get(p => p.EntityId == entityId).Select(p => p.Name).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
                return "";
            }
        }

        public static LocationFacade GetInstance()
        {
            return Facade;
        }
    }
}