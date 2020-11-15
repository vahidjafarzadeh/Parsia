using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.Role
{
    [ApiController]
    public class RoleService : ControllerBase
    {
        [HttpPost]
        [Route("service/role/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Role", "gridView");
            return checkAccess.Done
                ? RoleFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/role/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = RoleFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (RoleDto) dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Role",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? RoleFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/role/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Role", "update");
            return checkAccess.Done
                ? RoleFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/role/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Role", "delete");
            return checkAccess.Done
                ? RoleFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/role/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Role", "autocomplete");
            return checkAccess.Done
                ? RoleFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }

    }
}