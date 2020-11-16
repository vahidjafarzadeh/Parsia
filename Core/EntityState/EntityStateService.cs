using System;
using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.EntityState
{
    [ApiController]
    public class EntityStateService : ControllerBase
    {
        [HttpPost]
        [Route("service/entityState/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "EntityState", "gridView");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/entityState/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = EntityStateFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (EntityStateDto)dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "EntityState",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? EntityStateFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/entityState/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "EntityState", "update");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/entityState/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "EntityState", "delete");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/entityState/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "EntityState", "autocomplete");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }


        [HttpPost]
        [Route("service/entityState/lock")]
        public ServiceResult<object> Lock() {
            var ticket = HttpContext.Request.Form["ticket"];
            var userInfo = UserSessionManager.GetUserInfo(ticket);
            var bp = new BusinessParam(userInfo, new Clause());
            var dtoFromRequest = EntityStateFacade.GetInstance().GetDtoFromRequestWithCurrentUser(HttpContext.Request,bp);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (EntityStateDto)dtoFromRequest.Result;
            var checkAccess = UserSessionManager.CheckAccess(bp, "EntityState",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? EntityStateFacade.GetInstance().Save(bp, dto) : checkAccess;
        }
        [HttpPost]
        [Route("service/entityState/deleteLock")]
        public ServiceResult<object> DeleteLock()
        {
            var ticket = HttpContext.Request.Form["ticket"];
            var userInfo = UserSessionManager.GetUserInfo(ticket);
            var bp = new BusinessParam(userInfo, new Clause());
            var dtoFromRequest = EntityStateFacade.GetInstance().GetDtoFromRequestWithCurrentUser(HttpContext.Request,bp);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (EntityStateDto)dtoFromRequest.Result;
            var checkAccess = UserSessionManager.CheckAccess(bp, "EntityState", "delete");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().Delete(bp,dto)
                : checkAccess;
        }
        [HttpPost]
        [Route("service/entityState/getState")]
        public ServiceResult<object> GetState()
        {
            var ticket = HttpContext.Request.Form["ticket"];
            var userInfo = UserSessionManager.GetUserInfo(ticket);
            var bp = new BusinessParam(userInfo,new Clause());
            var dtoFromRequest = EntityStateFacade.GetInstance().GetDtoFromRequestWithCurrentUser(HttpContext.Request, bp);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (EntityStateDto)dtoFromRequest.Result;
            var checkAccess = UserSessionManager.CheckAccess(bp, "EntityState", "gridView");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().GetState(bp,dto)
                : checkAccess;
        }



    }
}