using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Base;
using DataLayer.Context;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Parsia.Core.Action;
using Parsia.Core.Elastic;
using Parsia.Core.UseCaseAction;

namespace Parsia.Core.UseCase
{
    public class UseCaseFacade : IBaseFacade<UseCaseDto>
    {
        private static readonly UseCaseFacade Facade = new UseCaseFacade();
        private static readonly UseCaseCopier Copier = new UseCaseCopier();

        private UseCaseFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.UseCase.UseCase>();
                var queryString = $"select entityId,useCaseName,clazz,tableName,uName as parent,ParentId,createBy,accessKey,fullTitle,deleted from (select * from (select EntityId as entityId, UseCaseName as useCaseName, Clazz as clazz, TableName as tableName, ParentId, CreateBy as createBy, FullTitle as fullTitle, AccessKey As accessKey, Deleted as deleted from {tableName}) as currentUseCase left join(select EntityId as eId, UseCaseName as uName from {tableName}) as parentUseCase on currentUseCase.ParentId = parentUseCase.eId) e" +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, "UseCase", false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
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
                        return new ServiceResult<object>(new List<UseCaseDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[] { "entityId", "useCaseName", "clazz", "tableName", "parent" };
                    list.Add(headerTitle);
                    list.AddRange(comboList);
                    return new ServiceResult<object>(list, comboList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UseCaseFacade.GridView", bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, UseCaseDto dto)
        {
            try
            {
                DataLayer.Model.Core.UseCase.UseCase useCase;
                if (dto.EntityId == 0)
                {
                    using (var unitOfWork = new UnitOfWork())
                    {
                        useCase = Copier.GetEntity(dto, bp, true);
                        unitOfWork.UseCase.Insert(useCase);
                        unitOfWork.UseCase.Save();
                    }

                    foreach (var item in dto.UseCaseActions)
                    {
                        item.UseCase = new UseCaseDto() { EntityId = useCase.EntityId };
                    }
                    var serviceResult = UseCaseActionFacade.GetInstance().SaveList(bp, dto.UseCaseActions);
                    if (!serviceResult.Done)
                    {
                        return serviceResult;
                    }
                }
                else
                {
                    var deletedList = UseCaseActionFacade.GetInstance().DeletedList(bp, dto.EntityId);
                    if (deletedList.Done)
                    {
                        foreach (var item in dto.UseCaseActions)
                        {
                            item.UseCase = new UseCaseDto() { EntityId = dto.EntityId };
                        }
                        var serviceResult = UseCaseActionFacade.GetInstance().SaveList(bp, dto.UseCaseActions);
                        if (serviceResult.Done)
                        {
                            using (var unitOfWork = new UnitOfWork())
                            {
                                useCase = Copier.GetEntity(dto, bp, false);
                                unitOfWork.UseCase.Update(useCase);
                                unitOfWork.UseCase.Save();
                            }
                        }
                        else
                        {
                            return serviceResult;
                        }
                    }
                    else
                    {
                        return deletedList;
                    }

                }
                Elastic<UseCaseDto, DataLayer.Model.Core.UseCase.UseCase>.SaveToElastic(useCase, "UseCase", bp);
                return new ServiceResult<object>(Copier.GetDto(useCase), 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UseCaseFacade.Save", bp.UserInfo);
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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "UseCaseFacade.ShowRow",
                        bp.UserInfo);
                using (var context = new ParsiContext())
                {
                    var useCase = context.UseCase.Where(p => p.EntityId == entityId)
                        .Include(p => p.CurrentUseCase)
                        .Include(p => p.ParentUseCaseUseCaseAction)
                        .Include(p => p.CreateUserEntity)
                        .Include(p => p.UpdateUserEntity)
                        .IgnoreQueryFilters()
                        .ToList();

                    return new ServiceResult<object>(Copier.GetDto(useCase[0]), 1);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UseCaseFacade.ShowRow", bp.UserInfo);
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
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "UseCaseFacade.Delete",
                        bp.UserInfo);
                DataLayer.Model.Core.UseCase.UseCase useCase;
                using (var unitOfWork = new UnitOfWork())
                {
                    useCase = unitOfWork.UseCase.GetRecord(entityId);
                }

                if (useCase == null)
                    return ExceptionUtil.ExceptionHandler("شناسه مورد نظر یافت نشد", "UseCaseFacade.Delete",
                        bp.UserInfo);

                useCase.Deleted = useCase.EntityId;
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.UseCase.Update(useCase);
                    unitOfWork.UseCase.Save();
                }
                Elastic<UseCaseDto, DataLayer.Model.Core.UseCase.UseCase>.SaveToElastic(useCase, "UseCase", bp);
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UseCaseFacade.Delete", bp.UserInfo);
            }
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.UseCase.UseCase>();
                var queryString = $"select * from (select EntityId as entityId,UseCaseName as useCaseName,Clazz as clazz,TableName as tableName,FullTitle as fullTitle,CreateBy as createBy,AccessKey as accessKey,Deleted as deleted from {tableName} ) e" +
                                  QueryUtil.GetWhereClause(bp.Clause,
                                      QueryUtil.GetConstraintForNativeQuery(bp, "UseCase", false, false, true)) +
                                  QueryUtil.GetOrderByClause(bp.Clause);
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboList = unitOfWork.UseCase.CreateNativeQuery(queryString, x => new UseCaseDto()
                    {
                        EntityId = Convert.ToInt64(x[0].ToString()),
                        UseCaseName = x[1]?.ToString(),
                        Clazz = x[2]?.ToString(),
                        TableName = x[3]?.ToString(),
                        FullTitle = x[4]?.ToString()
                    });
                    return comboList.Count == 0
                        ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد")
                        : new ServiceResult<object>(comboList, comboList.Count);
                }
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, "UseCaseFacade.AutocompleteView", bp.UserInfo);
            }
        }

        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new UseCaseDto();
            if (!string.IsNullOrEmpty(request.Form["EntityId"])) dto.EntityId = Convert.ToInt64(request.Form["EntityId"]);
            if (!string.IsNullOrEmpty(request.Form["useCaseName"])) dto.UseCaseName = request.Form["useCaseName"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام فرآیند را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["Code"])) dto.Code = request.Form["Code"];
            if (!string.IsNullOrEmpty(request.Form["clazz"])) dto.Clazz = request.Form["clazz"];
            if (!string.IsNullOrEmpty(request.Form["Active"])) dto.Active = Convert.ToBoolean(request.Form["Active"]);
            if (!string.IsNullOrEmpty(request.Form["virtualNode"])) dto.VirtualNode = Convert.ToBoolean(request.Form["virtualNode"]);
            if (!string.IsNullOrEmpty(request.Form["Ticket"])) dto.Ticket = request.Form["Ticket"];
            if (!string.IsNullOrEmpty(request.Form["tableName"])) dto.TableName = request.Form["tableName"];
            if (!string.IsNullOrEmpty(request.Form["parent"])) dto.Parent = new UseCaseDto() { EntityId = Convert.ToInt64(request.Form["parent"]) };
            if (!string.IsNullOrEmpty(request.Form["useCaseActions"]))
            {
                var usCaseAction = new List<long>();
                var strings = request.Form["useCaseActions"].ToString().Split(",");
                foreach (var item in strings)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        usCaseAction.Add(Convert.ToInt64(item));
                    }
                }
                var lstUseCaseActionDto = usCaseAction.Select(item => new UseCaseActionDto() { Action = new ActionDto() { EntityId = item } }).ToList();
                dto.UseCaseActions = lstUseCaseActionDto;
            }
            return new ServiceResult<object>(dto, 1);
        }
        public ServiceResult<object> GetTotalUseCase(BusinessParam bp, bool getAllData, string search, string pageNumber)
        {
            try
            {

                using (var context = new ParsiContext())
                {
                    var useCase = context.UseCase
                        .Include(p => p.CurrentUseCase)
                        .Include(p => p.ParentUseCaseUseCaseAction)
                        .ThenInclude(p => p.CurrentAction)
                        .ToList();
                    return new ServiceResult<object>(PrePareToShowInAccessGroup(Copier.GetDto(useCase)), 1);
                }


            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UseCaseFacade.ShowRow", bp.UserInfo);
            }
        }


        public static UseCaseFacade GetInstance()
        {
            return Facade;
        }

        private List<Dictionary<string, object>> PrePareToShowInAccessGroup(List<UseCaseDto> data)
        {
            var result = new List<Dictionary<string, object>>();
            foreach (var item in data)
            {
                var usecase = new Dictionary<string, object>()
                {
                    {"entityId",item.EntityId },
                    {"parent",item.Parent?.EntityId}, 
                    {"fullTitle","<i class='fas fa-star usecase'></i>"+item.FullTitle }
                };
                result.Add(usecase);
                foreach (var child in item.UseCaseActions)
                {
                    var action = new Dictionary<string, object>()
                    {
                        {"entityId",item.EntityId +"|"+child.Action.EntityId },
                        {"parent",item.EntityId},
                        {"fullTitle",child.Action.FullTitle }
                    };
                    result.Add(action);
                }
            }
            return result;
        }
    }
}