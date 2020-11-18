using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.Base;
using DataLayer.Tools;
using Datalayer.UnitOfWork;

namespace Parsia.Core.Action
{
    [ClassDetails(Clazz = "Action", Facade = "ActionFacade")]
    public class ActionFacade : IBaseFacade<ActionDto>
    {
        private static readonly ActionFacade Facade = new ActionFacade();
        private static readonly ActionCopier Copier = new ActionCopier();
        private static readonly ClassDetails[] ClassDetails = (ClassDetails[])typeof(ActionFacade).GetCustomAttributes(typeof(ClassDetails), true);

        private ActionFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                var tableName = Util.GetSqlServerTableName<DataLayer.Model.Core.Action.Action>();
                var queryString =
                    $"select * from (select EntityId as entityId,ActionName as actionName,ActionEnName as actionEnName,FullTitle as fullTitle,Deleted as deleted,AccessKey as accessKey,CreateBy as createBy from {tableName}) e  " +
                    QueryUtil.GetWhereClause(bp.Clause,
                        QueryUtil.GetConstraintForNativeQuery(bp, ClassDetails[0].Clazz, false, false, true)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                queryString = QueryUtil.SetPaging(bp.Clause.PageNo, bp.Clause.PageSize, queryString);
                using (var unitOfWork = new UnitOfWork())
                {
                    var actionList = unitOfWork.Action.CreateNativeQuery(queryString, x => new[]
                    {
                        x[0] != null ? Convert.ToInt32(x[0]) : (object) null,
                        x[1] != null ? Convert.ToInt64(x[1]) : (object) null,
                        x[2]?.ToString(),
                        x[3]?.ToString()
                    });
                    if (actionList.Count == 0)
                        return new ServiceResult<object>(new List<ActionDto>(), 0);
                    var list = new List<object>();
                    var headerTitle = new object[] { "entityId", "actionName", "actionEnName" };
                    list.Add(headerTitle);
                    list.AddRange(actionList);
                    return new ServiceResult<object>(list, actionList.Count);
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
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
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                List<DataLayer.Model.Core.Action.Action> myAction;
                using (var unitOfWork = new UnitOfWork())
                {
                    myAction = unitOfWork.Action.Get().ToList();
                }
                return new ServiceResult<object>(Copier.GetDto(myAction), myAction.Count);
            }
            catch (Exception ex)
            {
                return ExceptionUtil.ExceptionHandler(ex, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public static ActionFacade GetInstance()
        {
            return Facade;
        }
    }
}