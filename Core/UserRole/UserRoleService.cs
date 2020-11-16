using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.UserRole
{
    [ApiController]
    public class UserRoleService : ControllerBase
    {
        [HttpPost]
        [Route("service/userRole/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UserRole", "gridView");
            return checkAccess.Done
                ? UserRoleFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/userRole/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = UserRoleFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (UserRoleDto) dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UserRole",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? UserRoleFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/userRole/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UserRole", "update");
            return checkAccess.Done
                ? UserRoleFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/userRole/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UserRole", "delete");
            return checkAccess.Done
                ? UserRoleFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/userRole/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UserRole", "autocomplete");
            return checkAccess.Done
                ? UserRoleFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }

    }
}