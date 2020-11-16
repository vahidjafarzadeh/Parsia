using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.BusinessAccess
{
    [ApiController]
    public class BusinessAccessService : ControllerBase
    {
        [HttpPost]
        [Route("service/businessAccess/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "BusinessAccess", "gridView");
            return checkAccess.Done
                ? BusinessAccessFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/businessAccess/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = BusinessAccessFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (BusinessAccessDto)dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "BusinessAccess",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? BusinessAccessFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/businessAccess/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "BusinessAccess", "update");
            return checkAccess.Done
                ? BusinessAccessFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/businessAccess/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "BusinessAccess", "delete");
            return checkAccess.Done
                ? BusinessAccessFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/businessAccess/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "BusinessAccess", "autocomplete");
            return checkAccess.Done
                ? BusinessAccessFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
    }
}