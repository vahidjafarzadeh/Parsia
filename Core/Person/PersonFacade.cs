using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Base;
using DataLayer.Model.Core.User;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace Parsia.Core.Person
{
    public class PersonFacade : IBaseFacade<PersonDto>
    {
        private static readonly PersonFacade Facade = new PersonFacade();
        private static readonly PersonCopier Copier = new PersonCopier();
        private PersonFacade()
        {
        }
        public ServiceResult<object> GridView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Person.Person>();
                var queryString = "select * from (select ROW_NUMBER() OVER (ORDER BY entityId) as rowNumber,entityId,firstName,lastName,NationalCode,fatherName,birthDate,deleted,fullTitle,createBy,accessKey from "+tableName+") e " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, "Person", false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboList = unitOfWork.ComboVal.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString(),
                        x[4]?.ToString(),
                        x[5]?.ToString(),
                       (Util.GetTimeStamp(string.IsNullOrEmpty(x[6]?.ToString()) ?  (DateTime?) null : Convert.ToDateTime(x[6].ToString())))
                    });
                    if (comboList.Count == 0)
                        return new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد");
                    var list = new List<object>();
                    var headerTitle = new object[] { "entityId", "firstName", "lastName", "nationalId", "fatherName", "birthDate" };
                    list.Add(headerTitle);
                    list.AddRange(comboList);
                    return new ServiceResult<object>(list, comboList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "PersonFacade.GridView", bp.UserInfo);
            }
        }
        public ServiceResult<object> Save(BusinessParam bp, PersonDto dto)
        {
            try
            {
                DataLayer.Model.Core.Person.Person person;
                if (dto.EntityId == 0)
                    using (var unitOfWork = new UnitOfWork())
                    {
                        person = Copier.GetEntity(dto, bp, true);
                        unitOfWork.Person.Insert(person);
                        unitOfWork.Person.Save();
                    }
                else
                    using (var unitOfWork = new UnitOfWork())
                    {
                        person = Copier.GetEntity(dto, bp, false);
                        unitOfWork.Person.Update(person);
                        unitOfWork.Person.Save();
                    }

                return new ServiceResult<object>(Copier.GetDto(person), 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "PersonFacade.Save", bp.UserInfo);
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
                    var comboVal = new LinkedList<string>();

                    return comboVal == null
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکورد یافت نشد")
                        : new ServiceResult<object>(comboVal, 1);
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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "PersonFacade.Delete",
                        bp.UserInfo);
                DataLayer.Model.Core.Person.Person person;
                using (var unitOfWork = new UnitOfWork())
                {
                    person = unitOfWork.Person.GetRecord(entityId);
                }

                if (person == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "PersonFacade.Delete",
                        bp.UserInfo);

                person.Deleted = person.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Person.Update(person);
                    unitOfWork.Person.Save();
                }

                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "PersonFacade.Delete", bp.UserInfo);
            }
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Person.Person>();
                var queryString = "select * from (select EntityId,Name,Value,code,CreateBy,AccessKey,Deleted from " +
                                  tableName + " ) e" +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, "Person", true, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboList = unitOfWork.ComboVal.CreateNativeQuery(queryString, x => new PersonDto
                    {
                        EntityId = Convert.ToInt64(x[0].ToString()),
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
            var dto = new PersonDto();
            if (!string.IsNullOrEmpty(request.Form["EntityId"]))
                dto.EntityId = Convert.ToInt64(request.Form["EntityId"]);
            if (!string.IsNullOrEmpty(request.Form["Code"])) dto.Code = request.Form["Code"];
            if (!string.IsNullOrEmpty(request.Form["Active"])) dto.Active = Convert.ToBoolean(request.Form["Active"]);
            if (!string.IsNullOrEmpty(request.Form["Ticket"])) dto.Ticket = request.Form["Ticket"];
            return new ServiceResult<object>(dto, 1);
        }

        public static PersonFacade GetInstance()
        {
            return Facade;
        }
    }
}