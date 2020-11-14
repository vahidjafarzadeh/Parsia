using DataLayer.Base;
using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.User
{
    [ApiController]
    public class UserService : ControllerBase
    {
        [HttpPost]
        [Route("service/user/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Users", "gridView");
            return checkAccess.Done
                ? UserFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/user/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = UserFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (UserDto)dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Users",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? UserFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/user/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Users", "update");
            return checkAccess.Done
                ? UserFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/user/delete")]
        public ServiceResult<object> Delete(Clause clause)
        { 
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Users", "delete");
            return checkAccess.Done
                ? UserFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/user/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Users", "autocomplete");
            return checkAccess.Done
                ? UserFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
    }
}