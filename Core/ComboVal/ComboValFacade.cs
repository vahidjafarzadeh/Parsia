using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Base;
using DataLayer.Model.Core.User;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Parsia.Core.Elastic;

namespace Parsia.Core.ComboVal
{
    public class ComboValFacade : IBaseFacade<ComboValDto>
    {
        private static readonly ComboValFacade Facade = new ComboValFacade();
        private static readonly ComboValCopier Copier = new ComboValCopier();

        private ComboValFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.ComboVal.ComboVal>();
                var queryString =
                    "select entityId,name,value,code,parentId,fullTitle,createBy,accessKey from (select data.EntityId as entityId,data.Name as name,data.Value as value,data.Code as code,parent.Name as parentId,data.FullTitle as fullTitle,data.Deleted as deleted,data.CreateBy as createBy,data.AccessKey as accessKey from (select EntityId,Name,Value,Code,ParentId,FullTitle,Deleted,AccessKey,CreateBy from " +
                    tableName + " where Deleted =0) data left join(select EntityId, Name from " + tableName +
                    " where Deleted = 0) parent on data.ParentId = parent.EntityId ) e  " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, "ComboVal", false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo,bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboList = unitOfWork.ComboVal.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString(),
                        x[4]?.ToString(),
                        x[5]?.ToString()
                    });
                    if (comboList.Count == 0)
                        return new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد");
                    var list = new List<object>();
                    var headerTitle = new object[] {"entityId", "name", "value", "code", "parentName"};
                    list.Add(headerTitle);
                    list.AddRange(comboList);
                    return new ServiceResult<object>(list, comboList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "ComboValFacade.GridView", bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, ComboValDto dto)
        {
            try
            {
                DataLayer.Model.Core.ComboVal.ComboVal comboVal;
                if (dto.EntityId == 0)
                    using (var unitOfWork = new UnitOfWork())
                    {
                        comboVal = Copier.GetEntity(dto, bp, true);
                        unitOfWork.ComboVal.Insert(comboVal);
                        unitOfWork.ComboVal.Save();
                    }
                else
                    using (var unitOfWork = new UnitOfWork())
                    {
                        comboVal = Copier.GetEntity(dto, bp, false);
                        unitOfWork.ComboVal.Update(comboVal);
                        unitOfWork.ComboVal.Save();
                    }
                Elastic<ComboValDto, DataLayer.Model.Core.ComboVal.ComboVal>.SaveToElastic(comboVal, "ComboVal", bp);
                return new ServiceResult<object>(Copier.GetDto(comboVal), 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "ComboVal.Save", bp.UserInfo);
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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "ComboValFacade.ShowRow",
                        bp.UserInfo);
                var tableNameComboval = Util.GetSqlServerTableName<DataLayer.Model.Core.ComboVal.ComboVal>();
                var tableNameUser = Util.GetSqlServerTableName<Users>();
                var queryString = "select main.EntityId,main.Name,main.Value,main.AdminOnly,main.Active," +
                                  "main.Created,main.Updated,main.Code,combo.EntityId,combo.Name,created.EntityId," +
                                  "created.Username,updated.EntityId,updated.Username from (" +
                                  "select EntityId, Name, Value, ParentId, AdminOnly, Active, Created, Updated, Code, CreateBy, UpdateBy from " +
                                  tableNameComboval + ") main " +
                                  "left join(select EntityId, Name from " + tableNameComboval +
                                  ") combo on combo.EntityId = main.ParentId " +
                                  "left join(select EntityId, Username from " + tableNameUser +
                                  ") created on created.EntityId = main.CreateBy " +
                                  "left join(select EntityId, Username from " + tableNameUser +
                                  ") updated on updated.EntityId = main.UpdateBy " +
                                  "where main.EntityId = " + entityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboVal = unitOfWork.ComboVal.CreateNativeQuery(queryString, x => new ComboValDto
                    {
                        EntityId = Convert.ToInt64(x[0].ToString()),
                        Name = x[1]?.ToString(),
                        Value = x[2]?.ToString(),
                        AdminOnly = Convert.ToBoolean(x[3]?.ToString()),
                        Active = Convert.ToBoolean(x[4]?.ToString()),
                        Created = Util.GetTimeStamp(string.IsNullOrEmpty(x[5].ToString()) ? (DateTime?)null : Convert.ToDateTime(x[5]?.ToString())),
                        Updated = Util.GetTimeStamp(string.IsNullOrEmpty(x[6].ToString()) ? (DateTime?)null : Convert.ToDateTime(x[6]?.ToString())),
                        Code = x[7]?.ToString(),
                        Parent = string.IsNullOrEmpty(x[8].ToString())
                            ? null
                            : new ComboValDto {EntityId = Convert.ToInt64(x[8].ToString()), Name = x[9]?.ToString()},
                        CreatedBy = string.IsNullOrEmpty(x[10].ToString())
                            ? null
                            : new UserDto {EntityId = Convert.ToInt64(x[10].ToString()), Username = x[11]?.ToString()},
                        UpdatedBy = string.IsNullOrEmpty(x[12].ToString())
                            ? null
                            : new UserDto {EntityId = Convert.ToInt64(x[12].ToString()), Username = x[13]?.ToString()}
                    });

                    return comboVal == null
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکورد یافت نشد")
                        : new ServiceResult<object>(comboVal[0], 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "ComboValFacade.ShowRow", bp.UserInfo);
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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "ComboValFacade.Delete",
                        bp.UserInfo);
                DataLayer.Model.Core.ComboVal.ComboVal comboVal;
                using (var unitOfWork = new UnitOfWork())
                {
                    comboVal = unitOfWork.ComboVal.GetRecord(entityId);
                }

                if (comboVal == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "ComboValFacade.Delete",
                        bp.UserInfo);

                comboVal.Deleted = comboVal.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.ComboVal.Update(comboVal);
                    unitOfWork.ComboVal.Save();
                }
                Elastic<ComboValDto, DataLayer.Model.Core.ComboVal.ComboVal>.SaveToElastic(comboVal, "ComboVal", bp);
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "ComboValFacade.Delete", bp.UserInfo);
            }
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.ComboVal.ComboVal>();
                var queryString = "select * from (select EntityId,Name,Value,code,CreateBy,AccessKey,Deleted from " +
                                  tableName + " ) e" +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, "ComboVal", false, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboList = unitOfWork.ComboVal.CreateNativeQuery(queryString, x => new ComboValDto
                    {
                        EntityId = Convert.ToInt64(x[0].ToString()),
                        Name = x[1]?.ToString(),
                        Value = x[2]?.ToString(),
                        Code = x[3]?.ToString()
                    });
                    return comboList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(comboList, comboList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, "ComboValFacade.AutocompleteView", bp.UserInfo);
            }
        }
        public ServiceResult<object> AutocompleteViewParent(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.ComboVal.ComboVal>();
                var queryString = $"SELECT * FROM(SELECT e1.EntityId AS entityId,e1.Name AS name,e1.Value AS value,e1.code AS code,e2.Name AS parentName,e2.code AS parentCode,e1.Deleted,e1.FullTitle AS fullTitle,e1.AccessKey,e1.CreateBy FROM {tableName} e1 LEFT JOIN {tableName} e2 on e1.ParentId = e2.EntityId where e1.Deleted = 0 and e2.Deleted = 0) e" + QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, "ComboVal", false, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboList = unitOfWork.ComboVal.CreateNativeQuery(queryString, x => new ComboValDto
                    {
                        EntityId = Convert.ToInt64(x[0].ToString()),
                        Name = x[1]?.ToString(),
                        Value = x[2]?.ToString(),
                        Code = x[3]?.ToString()
                    });
                    return comboList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(comboList, comboList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, "ComboValFacade.AutocompleteView", bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new ComboValDto();
            if (!string.IsNullOrEmpty(request.Form["EntityId"]))
                dto.EntityId = Convert.ToInt64(request.Form["EntityId"]);
            if (!string.IsNullOrEmpty(request.Form["Name"]))
                dto.Name = request.Form["Name"];
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["Code"])) dto.Code = request.Form["Code"];
            if (!string.IsNullOrEmpty(request.Form["Value"])) dto.Value = request.Form["Value"];
            if (!string.IsNullOrEmpty(request.Form["Active"])) dto.Active = Convert.ToBoolean(request.Form["Active"]);
            if (!string.IsNullOrEmpty(request.Form["AdminOnly"]))
                dto.AdminOnly = Convert.ToBoolean(request.Form["AdminOnly"]);
            if (!string.IsNullOrEmpty(request.Form["Parent"]))
                dto.Parent = new ComboValDto {EntityId = Convert.ToInt64(request.Form["Parent"])};
            if (!string.IsNullOrEmpty(request.Form["Ticket"])) dto.Ticket = request.Form["Ticket"];
            return new ServiceResult<object>(dto, 1);
        }

        public static ComboValFacade GetInstance()
        {
            return Facade;
        }
    }
}