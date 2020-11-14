using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Base;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Exception = System.Exception;

namespace Parsia.Core.Action
{
    public class ActionFacade : IBaseFacade<ActionDto>
    {
        private static readonly ActionFacade Facade = new ActionFacade();
        private static readonly ActionCopier Copier = new ActionCopier();
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        private ActionFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Action.Action>();
                var queryString =
                    $"select * from (select EntityId as entityId,ActionName as actionName,ActionEnName as actionEnName,FullTitle as fullTitle,Deleted as deleted,AccessKey as accessKey,CreateBy as createBy from {tableName}) e  " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, "Action", false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var comboList = unitOfWork.ComboVal.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString()
                    });
                    if (comboList.Count == 0)
                        return new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "رکوردی یافت نشد");
                    var list = new List<object>();
                    var headerTitle = new object[] { "entityId", "actionName", "actionEnName"};
                    list.Add(headerTitle);
                    list.AddRange(comboList);
                    return new ServiceResult<object>(list, comboList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "ActionFacade.GridView", bp.UserInfo);
            }
        }

        public ServiceResult<object> Save(BusinessParam bp, ActionDto dto)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> ShowRow(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> Delete(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }
        public ServiceResult<object> GetAllList(BusinessParam bp)
        {
            try
            {
                List<DataLayer.Model.Core.Action.Action> myAction;
                using (var unitOfWork = new UnitOfWork())
                {
                     myAction = unitOfWork.Action.Get().ToList();
                }
                return new ServiceResult<object>(Copier.GetDto(myAction),myAction.Count);
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, "ActionFacade.GetAllList", bp.UserInfo);
            }
        }

        public static ActionFacade GetInstance()
        {
            return Facade;
        }
    }
}