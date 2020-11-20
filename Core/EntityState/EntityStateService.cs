using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.EntityState
{
    [ApiController]
    [ClassDetails(Clazz = "EntityState", Facade = "EntityStateService")]
    public class EntityStateService : ControllerBase
    {
        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(EntityStateFacade).GetCustomAttributes(typeof(ClassDetails), true);

        private readonly IUserSessionManager _userSessionManager;

        public EntityStateService(IUserSessionManager userSessionManager)
        {
            _userSessionManager = userSessionManager;
        }

        [HttpPost]
        [Route("service/entityState/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "gridView");
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
            var dto = (EntityStateDto) dtoFromRequest.Result;
            var userInfo = _userSessionManager.GetUserInfo(dto.Ticket, Request);
            var bp = new BusinessParam(userInfo);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz,
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? EntityStateFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/entityState/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "update");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/entityState/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "delete");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/entityState/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "autocomplete");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }


        [HttpPost]
        [Route("service/entityState/lock")]
        public ServiceResult<object> Lock()
        {
            var ticket = HttpContext.Request.Form["ticket"];
            var userInfo = _userSessionManager.GetUserInfo(ticket, Request);
            var bp = new BusinessParam(userInfo, new Clause());
            var dtoFromRequest = EntityStateFacade.GetInstance()
                .GetDtoFromRequestWithCurrentUser(HttpContext.Request, bp);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (EntityStateDto) dtoFromRequest.Result;
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz,
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? EntityStateFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/entityState/deleteLock")]
        public ServiceResult<object> DeleteLock()
        {
            var ticket = HttpContext.Request.Form["ticket"];
            var userInfo = _userSessionManager.GetUserInfo(ticket, Request);
            var bp = new BusinessParam(userInfo, new Clause());
            var dtoFromRequest = EntityStateFacade.GetInstance()
                .GetDtoFromRequestWithCurrentUser(HttpContext.Request, bp);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (EntityStateDto) dtoFromRequest.Result;
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "delete");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().Delete(bp, dto)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/entityState/getState")]
        public ServiceResult<object> GetState()
        {
            var ticket = HttpContext.Request.Form["ticket"];
            var userInfo = _userSessionManager.GetUserInfo(ticket, Request);
            var bp = new BusinessParam(userInfo, new Clause());
            var dtoFromRequest = EntityStateFacade.GetInstance()
                .GetDtoFromRequestWithCurrentUser(HttpContext.Request, bp);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (EntityStateDto) dtoFromRequest.Result;
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "gridView");
            return checkAccess.Done
                ? EntityStateFacade.GetInstance().GetState(bp, dto)
                : checkAccess;
        }
    }
}