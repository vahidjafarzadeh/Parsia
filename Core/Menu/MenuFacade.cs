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
using Parsia.Core.UseCase;

namespace Parsia.Core.Menu
{
    [ClassDetails(Clazz = "Menu", Facade = "MenuFacade")]
    public class MenuFacade : IBaseFacade<MenuDto>
    {
        private static readonly MenuFacade Facade = new MenuFacade();
        private static readonly MenuCopier Copier = new MenuCopier();
        private static readonly ClassDetails[] ClassDetails = (ClassDetails[])typeof(MenuFacade).GetCustomAttributes(typeof(ClassDetails), true);

        private MenuFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Menu.Menu>();
                var tblUseCase = Util.GetSqlServerTableName<DataLayer.Model.Core.UseCase.UseCase>();
                var queryString = $"select entityId,useCaseName,name,title,orderNode,parentName,useCase,parentId,deleted,createBy,fullTitle,accessKey from ( select EntityId as entityId, Name as name,Title as title, OrderNode as orderNode,useCaseTarget.useCaseName as useCaseName ,parentMenu.parentName as parentName, ParentId as parentId, UseCase as useCase,Deleted as deleted , CreateBy as createBy , AccessKey as accessKey, FullTitle as fullTitle from {tableName} as mainData left join (select EntityId as useCaseEntityId,UseCaseName as useCaseName from {tblUseCase}) as useCaseTarget on useCaseTarget.useCaseEntityId = mainData.UseCase left join (select EntityId as parentEntityId , Name as parentName  from {tableName}) as parentMenu on parentMenu.parentEntityId = mainData.ParentId ) e " +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var lstMenu = unitOfWork.Menu.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString(),
                        x[4]?.ToString(),
                        x[5]?.ToString(),
                        x[6]?.ToString()
                    });
                    if (lstMenu.Count == 0)
                        return new ServiceResult<object>(new List<MenuDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[] { "entityId", "useCase", "name", "title", "orderNode", "parent" };
                    list.Add(headerTitle);
                    list.AddRange(lstMenu);
                    return new ServiceResult<object>(list, lstMenu.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }
        public ServiceResult<object> Save(BusinessParam bp, MenuDto dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                DataLayer.Model.Core.Menu.Menu menu;
                if (dto.EntityId == 0)

                    using (var unitOfWork = new UnitOfWork())
                    {
                        menu = Copier.GetEntity(dto, bp, true);
                        unitOfWork.Menu.Insert(menu);
                        unitOfWork.Menu.Save();
                    }
                else
                    using (var unitOfWork = new UnitOfWork())
                    {
                        menu = Copier.GetEntity(dto, bp, false);
                        unitOfWork.Menu.Update(menu);
                        unitOfWork.Menu.Save();
                    }

                Elastic<MenuDto, DataLayer.Model.Core.Menu.Menu>.SaveToElastic(menu, ClassDetails[0].Clazz, bp);
                return new ServiceResult<object>(Copier.GetDto(menu), 1);
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
                    var menu = context.Menu.Where(p => p.EntityId == entityId)
                        .Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity)
                        .Include(p => p.CurrentFile)
                        .Include(p => p.CurrentMenu)
                        .Include(p => p.CurrentTargetComboVal)
                        .Include(p => p.CurrentUseCase)
                        .IgnoreQueryFilters()
                        .ToList();
                    return new ServiceResult<object>(Copier.GetDto(menu[0]), 1);
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
                DataLayer.Model.Core.Menu.Menu menu;
                using (var unitOfWork = new UnitOfWork())
                {
                    menu = unitOfWork.Menu.GetRecord(entityId);
                }

                if (menu == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", ClassDetails[0].Facade + methodName,
                        bp.UserInfo);

                menu.Deleted = menu.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Menu.Update(menu);
                    unitOfWork.Menu.Save();
                }
                Elastic<MenuDto, DataLayer.Model.Core.Menu.Menu>.SaveToElastic(menu, ClassDetails[0].Clazz, bp);
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
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Menu.Menu>();
                var queryString = $"select * from (select EntityId as entityId,Name as name,Title as title,Path as path,OrderNode as orderNode,FullTitle as fullTitle,Deleted as deleted,AccessKey as accessKey,CreateBy as createBy from {tableName}) e " +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, true, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var menuList = unitOfWork.Menu.CreateNativeQuery(queryString, x => new Dictionary<string, object>()
                    {
                        {"entityId",Convert.ToInt64(x[0].ToString()) },
                        {"name",x[1]?.ToString() },
                        {"title",x[2]?.ToString() },
                        {"path",x[3]?.ToString() },
                        {"orderNode",x[4]?.ToString() },
                        {"fullTitle",x[5]?.ToString() }
                    });
                    return menuList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(menuList, menuList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> GetAllMenu(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                using (var context = new ParsiContext())
                {
                    var list = new List<MenuDto>();
                    var data = context.Menu.Include(p => p.CurrentUseCase).ToList();
                    foreach (var menu in data)
                    {
                        var usecase = menu.CurrentUseCase.Clazz.ToLower();
                        if (bp.UserInfo.RoleId == DataLayer.Tools.SystemConfig.SystemRoleId)
                        {
                            list.Add(Copier.GetDto(menu));
                        }
                        else if (bp.UserInfo.AccessUserInfos.UseCase.ContainsKey(usecase))
                        {
                            var lst = bp.UserInfo.AccessUserInfos.UseCase[usecase];
                            foreach (var item in lst)
                            {
                                if (item.ToLower() == "showinmenu")
                                {
                                    list.Add(Copier.GetDto(menu));
                                }
                            }
                        }
                    }
                    return new ServiceResult<object>(list, list.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new MenuDto();
            if (!string.IsNullOrEmpty(request.Form["entityId"])) dto.EntityId = Convert.ToInt64(request.Form["entityId"]);
            if (!string.IsNullOrEmpty(request.Form["name"])) dto.Name = request.Form["name"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام منو را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["title"])) dto.Title = request.Form["title"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا عنوان منو را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["orderNode"])) dto.OrderNode = Convert.ToInt32(request.Form["orderNode"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا سطح منو را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["path"])) dto.Path = request.Form["path"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا مسیر منو را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["icon"])) dto.Icon = request.Form["icon"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا آیکون منو را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["useCase"])) dto.UseCase = new UseCaseDto() { EntityId = Convert.ToInt64(request.Form["useCase"]) }; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا فرآیند منو را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["target"])) dto.Target = new ComboValDto() { EntityId = Convert.ToInt64(request.Form["target"]) }; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نحوه نمایش منو را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["parent"])) dto.Parent = new MenuDto() { EntityId = Convert.ToInt64(request.Form["parent"]) };
            if (!string.IsNullOrEmpty(request.Form["file"])) dto.File = new FileDto() { EntityId = Convert.ToInt64(request.Form["file"]) };
            if (!string.IsNullOrEmpty(request.Form["ticket"])) dto.Ticket = request.Form["ticket"];
            if (!string.IsNullOrEmpty(request.Form["code"])) dto.Code = request.Form["code"];
            if (!string.IsNullOrEmpty(request.Form["active"])) dto.Active = Convert.ToBoolean(request.Form["active"]);
            return new ServiceResult<object>(dto, 1);
        }

        public static MenuFacade GetInstance()
        {
            return Facade;
        }
    }
}