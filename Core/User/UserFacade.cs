using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Base;
using DataLayer.Context;
using DataLayer.Model.Core.User;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Parsia.Core.Elastic;
using Parsia.Core.Person;
using System.Security.Cryptography;
using System.Text;

namespace Parsia.Core.User
{
    public class UserFacade : IBaseFacade<UserDto>
    {
        private static readonly UserFacade Facade = new UserFacade();
        private static readonly UserCopier Copier = new UserCopier();

        private UserFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.User.Users>();
                var queryString = $"select entityId,firstName,lastName,username,attempt,lastVisit,deleted,fullTitle,createBy,accessKey from (select EntityId as entityId, FirstName as firstName,LastName as lastName,Username as username,Attempt as attempt,LastVisit as lastVisit,FullTitle as fullTitle,Deleted as deleted,CreateBy as createBy,AccessKey as accessKey from {tableName} where code <> 'ADMIN') e " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, "Users", false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboList = unitOfWork.Users.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt32(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString(),
                        x[4]?.ToString(),
                        x[5]?.ToString(),
                        (Util.GetTimeStamp(string.IsNullOrEmpty(x[6]?.ToString()) ?  (DateTime?) null : Convert.ToDateTime(x[6].ToString())))
                    });
                    if (comboList.Count == 0)
                        return new ServiceResult<object>(new List<PersonDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[] { "entityId", "firstName", "lastName", "username", "attempt", "lastVisit" };
                    list.Add(headerTitle);
                    list.AddRange(comboList);
                    return new ServiceResult<object>(list, comboList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UserFacade.GridView", bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, UserDto dto)
        {
            try
            {
                DataLayer.Model.Core.User.Users users;
                if (dto.EntityId == 0)
                {
                    var checkUserExist = CheckUserExist(bp, dto.Username);
                    if (!checkUserExist.Done)
                    {
                        return checkUserExist;
                    }
                    else if (checkUserExist.Done && checkUserExist.ResultCountAll == 1)
                    {
                        return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربر گرامی با عرض پوزش این نام کاربری از قبل موجود می باشد");
                    }
                    else
                    {
                        dto.Password = GetHashPassword(dto.Password);
                        using (var unitOfWork = new UnitOfWork())
                        {
                            users = Copier.GetEntity(dto, bp, true);
                            unitOfWork.Users.Insert(users);
                            unitOfWork.Users.Save();
                        }
                    }

                }
                else
                {
                    var checkUserExist = CheckUserExist(bp, dto.Username);
                    if (!checkUserExist.Done)
                    {
                        return checkUserExist;
                    }
                    else if (checkUserExist.Done && checkUserExist.ResultCountAll == 1)
                    {
                        var result = (Users)checkUserExist.Result;
                        if (result.EntityId != dto.EntityId)
                        {
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربر گرامی با عرض پوزش این نام کاربری از قبل موجود می باشد");
                        }
                        else
                        {
                            if (result.Password != dto.Password)
                            {
                                dto.Password = GetHashPassword(dto.Password);
                            }
                            using (var unitOfWork = new UnitOfWork())
                            {
                                users = Copier.GetEntity(dto, bp, false);
                                unitOfWork.Users.Update(users);
                                unitOfWork.Users.Save();
                            }
                        }
                    }
                    else
                    {
                        dto.Password = GetHashPassword(dto.Password);
                        using (var unitOfWork = new UnitOfWork())
                        {
                            users = Copier.GetEntity(dto, bp, false);
                            unitOfWork.Users.Update(users);
                            unitOfWork.Users.Save();
                        }
                    }

                }

                Elastic<UserDto, DataLayer.Model.Core.User.Users>.SaveToElastic(users, "Users", bp);
                return new ServiceResult<object>(Copier.GetDto(users), 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UserFacade.Save", bp.UserInfo);
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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "UserFacade.ShowRow",
                        bp.UserInfo);
                using (var context = new ParsiContext())
                {
                    var user = context.Users.Where(p => p.EntityId == entityId)
                        .Include(p => p.CurrentPerson)
                        .Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity)
                        .ToList();
                    return user == null
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکورد یافت نشد")
                        : new ServiceResult<object>(Copier.GetDto(user[0]), 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UserFacade.ShowRow", bp.UserInfo);
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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "UserFacade.Delete",
                        bp.UserInfo);
                DataLayer.Model.Core.User.Users users;
                using (var unitOfWork = new UnitOfWork())
                {
                    users = unitOfWork.Users.GetRecord(entityId);
                }

                if (users == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "UserFacade.Delete",
                        bp.UserInfo);

                users.Deleted = users.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Users.Update(users);
                    unitOfWork.Users.Save();
                }
                Elastic<UserDto, DataLayer.Model.Core.User.Users>.SaveToElastic(users, "Users", bp);
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UserFacade.Delete", bp.UserInfo);
            }
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.User.Users>();
                var queryString = $"select * from (select EntityId as entityId,PersonId as parentId,FirstName as firstName,LastName as lastName,Username as username, FullTitle as fullTitle,Deleted as deleted,CreateBy as createBy,AccessKey as accessKey from {tableName}) e" +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, "Users", true, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var entityList = unitOfWork.ComboVal.CreateNativeQuery(queryString, x => new Dictionary<string, object>()
                    {
                        {"entityId",Convert.ToInt64(x[0].ToString()) },
                        {"parentId",(!string.IsNullOrEmpty(x[1].ToString())?Convert.ToInt64(x[1].ToString()):(long?) null) },
                        {"firstName",x[2]?.ToString() },
                        {"lastName",x[3]?.ToString() },
                        {"username",x[4]?.ToString() },
                        {"fullTitle",x[4]?.ToString() }
                    });
                    return entityList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(entityList, entityList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, "UserFacade.AutocompleteView", bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new UserDto();
            if (!string.IsNullOrEmpty(request.Form["firstName"])) dto.FirstName = request.Form["firstName"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["person"])) dto.PersonId = Convert.ToInt64(request.Form["person"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا فرد را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["firstName"])) dto.FirstName = request.Form["firstName"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["lastName"])) dto.LastName = request.Form["lastName"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام خانوادگی را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["username"])) dto.Username = request.Form["username"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام کاربری را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["password"])) dto.Password = request.Form["password"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا گذرواژه را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["emailCode"])) dto.EmailCode = request.Form["emailCode"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد فعال سازی ایمیل  را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["phoneCode"])) dto.PhoneCode = request.Form["phoneCode"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد فعال سازی موبایل را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["attempt"])) dto.Attempt = Convert.ToInt16(request.Form["attempt"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا تعداد تلاش ناموفق برای ورود را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["lastVisit"])) dto.LastVisit = Util.GetDateTime(Convert.ToDouble(request.Form["lastVisit"])); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا آخرین بازدید را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["EntityId"])) dto.EntityId = Convert.ToInt64(request.Form["EntityId"]);
            if (!string.IsNullOrEmpty(request.Form["Code"])) dto.Code = request.Form["Code"];
            if (!string.IsNullOrEmpty(request.Form["Active"])) dto.Active = Convert.ToBoolean(request.Form["Active"]);
            if (!string.IsNullOrEmpty(request.Form["Ticket"])) dto.Ticket = request.Form["Ticket"];
            if (dto.Username.Length < 8)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "نام کاربری باید حداقل 8 کاراکتر باشد");
            }
            if (dto.Password.Length < 8)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "گذرواژه باید حداقل 8 کاراکتر باشد");
            }
            return new ServiceResult<object>(dto, 1);
        }

        public static UserFacade GetInstance()
        {
            return Facade;
        }


        private ServiceResult<object> CheckUserExist(BusinessParam bp, string username)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var firstOrDefault = unitOfWork.Users.Get(p => p.Username.ToLower().Equals(username.ToLower())).FirstOrDefault();
                    return firstOrDefault == null ? new ServiceResult<object>(null, 0) : new ServiceResult<object>(firstOrDefault, 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UserFacade.CheckUserExist", bp.UserInfo);
            }
        }

        private string GetHashPassword(string password)
        {
            var sha1 = MD5.Create();
            var step1 = Encoding.UTF8.GetBytes(password);
            var step2 = sha1.ComputeHash(step1);
            var step3 = Util.BinaryToHex(step2);
            return step3;
        }
    }
}