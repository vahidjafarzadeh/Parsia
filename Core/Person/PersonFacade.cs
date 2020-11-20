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
using Parsia.Core.File;

namespace Parsia.Core.Person
{
    [ClassDetails(Clazz = "Person", Facade = "PersonFacade")]
    public class PersonFacade : IBaseFacade<PersonDto>
    {
        private static readonly PersonFacade Facade = new PersonFacade();
        private static readonly PersonCopier Copier = new PersonCopier();

        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(PersonFacade).GetCustomAttributes(typeof(ClassDetails), true);

        private PersonFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Person.Person>();
                var queryString =
                    $"select * from (select entityId,firstName,lastName,NationalCode,fatherName,birthDate,deleted,fullTitle,createBy,accessKey from {tableName} where Code <> 'ADMIN') e " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var personList = unitOfWork.Person.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString(),
                        x[4]?.ToString(),
                        x[5]?.ToString(),
                        Util.GetTimeStamp(string.IsNullOrEmpty(x[6]?.ToString())
                            ? (DateTime?) null
                            : Convert.ToDateTime(x[6].ToString()))
                    });
                    if (personList.Count == 0)
                        return new ServiceResult<object>(new List<PersonDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[]
                        {"entityId", "firstName", "lastName", "nationalCode", "fatherName", "birthDate"};
                    list.Add(headerTitle);
                    list.AddRange(personList);
                    return new ServiceResult<object>(list, personList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, PersonDto dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
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

                Elastic<PersonDto, DataLayer.Model.Core.Person.Person>.SaveToElastic(person, ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(Copier.GetDto(person), 1);
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
                    var person = context.Person.Where(p => p.EntityId == entityId)
                        .Include(p => p.CurrentFile)
                        .Include(p => p.CurrentBloodType)
                        .Include(p => p.CurrentCitizenship)
                        .Include(p => p.CurrentDisabilityType)
                        .Include(p => p.CurrentHealthStatus)
                        .Include(p => p.CurrentHousingSituation)
                        .Include(p => p.CurrentLifeStatus)
                        .Include(p => p.CurrentMaritalStatus)
                        .Include(p => p.CurrentMilitaryServiceStatus)
                        .Include(p => p.CurrentNationality)
                        .Include(p => p.CurrentSex)
                        .Include(p => p.CurrentReligion)
                        .Include(p => p.CurrentSubReligion)
                        .Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity)
                        .IgnoreQueryFilters()
                        .ToList();
                    return person.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکورد یافت نشد")
                        : new ServiceResult<object>(Copier.GetDto(person[0]), 1);
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
                DataLayer.Model.Core.Person.Person person;
                using (var unitOfWork = new UnitOfWork())
                {
                    person = unitOfWork.Person.GetRecord(entityId);
                }

                if (person == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد",
                        ClassDetails[0].Facade + methodName,
                        bp.UserInfo);

                person.Deleted = person.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Person.Update(person);
                    unitOfWork.Person.Save();
                }

                Elastic<PersonDto, DataLayer.Model.Core.Person.Person>.SaveToElastic(person, ClassDetails[0].Clazz, bp);
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
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Person.Person>();
                var queryString =
                    $"select * from (select EntityId as entityId,FirstName as firstName,LastName as lastName,FullTitle as fullTitle,Deleted as deleted,AccessKey as accessKey,CreateBy as createBy from {tableName}) e" +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, true, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var personList = unitOfWork.Person.CreateNativeQuery(queryString, x =>
                        new Dictionary<string, object>
                        {
                            {"entityId", Convert.ToInt64(x[0].ToString())},
                            {"fullName", $"{x[1]} {x[2]}"},
                            {"firstName", $"{x[1]}"},
                            {"lastName", $"{x[2]}"},
                            {"fullTitle", x[3]?.ToString()}
                        });
                    return personList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(personList, personList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new PersonDto();
            if (!string.IsNullOrEmpty(request.Form["firstName"])) dto.FirstName = request.Form["firstName"];
            else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["lastName"])) dto.LastName = request.Form["lastName"];
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                    "لطفا نام خانوادگی را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["nationalCode"])) dto.NationalCode = request.Form["nationalCode"];
            else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد ملی را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["birthPlace"])) dto.BirthPlace = request.Form["birthPlace"];
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا محل تولد را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["birthDate"]))
                dto.BirthDate = Convert.ToDouble(request.Form["birthDate"]);
            else
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                    "لطفا تاریخ تولد را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["EntityId"]))
                dto.EntityId = Convert.ToInt64(request.Form["EntityId"]);
            if (!string.IsNullOrEmpty(request.Form["Active"])) dto.Active = Convert.ToBoolean(request.Form["Active"]);
            if (!string.IsNullOrEmpty(request.Form["Ticket"])) dto.Ticket = request.Form["Ticket"];
            if (!string.IsNullOrEmpty(request.Form["persianCode"])) dto.PersianCode = request.Form["persianCode"];
            if (!string.IsNullOrEmpty(request.Form["fatherName"])) dto.FatherName = request.Form["fatherName"];
            if (!string.IsNullOrEmpty(request.Form["motherName"])) dto.MotherName = request.Form["motherName"];
            if (!string.IsNullOrEmpty(request.Form["exportationPlace"]))
                dto.ExportationPlace = request.Form["exportationPlace"];
            if (!string.IsNullOrEmpty(request.Form["certificateSeries"]))
                dto.CertificateSeries = request.Form["certificateSeries"];
            if (!string.IsNullOrEmpty(request.Form["identitySerialNumber"]))
                dto.IdentitySerialNumber = request.Form["identitySerialNumber"];
            if (!string.IsNullOrEmpty(request.Form["mobile"])) dto.Mobile = request.Form["mobile"];
            if (!string.IsNullOrEmpty(request.Form["emergencyPhone"]))
                dto.EmergencyPhone = request.Form["emergencyPhone"];
            if (!string.IsNullOrEmpty(request.Form["email"])) dto.Email = request.Form["email"];
            if (!string.IsNullOrEmpty(request.Form["code"])) dto.Code = request.Form["code"];
            if (!string.IsNullOrEmpty(request.Form["leftHanded"]))
                dto.LeftHanded = Convert.ToBoolean(request.Form["leftHanded"]);
            if (!string.IsNullOrEmpty(request.Form["sex"]))
                dto.Sex = new ComboValDto {EntityId = Convert.ToInt64(request.Form["sex"])};
            if (!string.IsNullOrEmpty(request.Form["nationality"]))
                dto.Nationality = new ComboValDto {EntityId = Convert.ToInt64(request.Form["nationality"])};
            if (!string.IsNullOrEmpty(request.Form["bloodType"]))
                dto.BloodType = new ComboValDto {EntityId = Convert.ToInt64(request.Form["bloodType"])};
            if (!string.IsNullOrEmpty(request.Form["lifeStatus"]))
                dto.LifeStatus = new ComboValDto {EntityId = Convert.ToInt64(request.Form["lifeStatus"])};
            if (!string.IsNullOrEmpty(request.Form["citizenship"]))
                dto.Citizenship = new ComboValDto {EntityId = Convert.ToInt64(request.Form["citizenship"])};
            if (!string.IsNullOrEmpty(request.Form["religion"]))
                dto.Religion = new ComboValDto {EntityId = Convert.ToInt64(request.Form["religion"])};
            if (!string.IsNullOrEmpty(request.Form["subReligion"]))
                dto.SubReligion = new ComboValDto {EntityId = Convert.ToInt64(request.Form["subReligion"])};
            if (!string.IsNullOrEmpty(request.Form["maritalStatus"]))
                dto.MaritalStatus = new ComboValDto {EntityId = Convert.ToInt64(request.Form["maritalStatus"])};
            if (!string.IsNullOrEmpty(request.Form["militaryServiceStatus"]))
                dto.MilitaryServiceStatus = new ComboValDto
                    {EntityId = Convert.ToInt64(request.Form["militaryServiceStatus"])};
            if (!string.IsNullOrEmpty(request.Form["housingSituation"]))
                dto.HousingSituation = new ComboValDto {EntityId = Convert.ToInt64(request.Form["housingSituation"])};
            if (!string.IsNullOrEmpty(request.Form["healthStatus"]))
                dto.HealthStatus = new ComboValDto {EntityId = Convert.ToInt64(request.Form["healthStatus"])};
            if (!string.IsNullOrEmpty(request.Form["description"])) dto.Description = request.Form["description"];
            if (!string.IsNullOrEmpty(request.Form["file"]))
                dto.File = new FileDto {EntityId = Convert.ToInt64(request.Form["file"])};
            return new ServiceResult<object>(dto, 1);
        }

        public static PersonFacade GetInstance()
        {
            return Facade;
        }
    }
}