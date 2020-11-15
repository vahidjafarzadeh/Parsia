using System.Collections.Generic;
using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;
using Parsia.Core.UseCaseActionAccessGroup;

namespace Parsia.Core.AccessGroup
{
    [ApiController]
    public class AccessGroupService : ControllerBase
    {
        [HttpPost]
        [Route("service/accessGroup/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "AccessGroup", "gridView");
            return checkAccess.Done
                ? AccessGroupFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/accessGroup/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = AccessGroupFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (List<UseCaseActionAccessGroupDto>)dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto[0].AccessGroup.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "AccessGroup",
                dto[0].AccessGroup.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? AccessGroupFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/accessGroup/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "AccessGroup", "update");
            return checkAccess.Done
                ? AccessGroupFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/accessGroup/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "AccessGroup", "delete");
            return checkAccess.Done
                ? AccessGroupFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/accessGroup/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "AccessGroup", "autocomplete");
            return checkAccess.Done
                ? AccessGroupFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
        
        [HttpPost]
        [Route("service/accessGroup/getAllData")]
        public ServiceResult<object> GetAllData(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "AccessGroup", "gridView");
            return checkAccess.Done
                ? AccessGroupFacade.GetInstance().GetAllData(bp)
                : checkAccess;
        }
    }
}