using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.UserRole
{
    [ApiController]
    public class UserRoleService : ControllerBase
    {
        private readonly IUserSessionManager _userSessionManager;

        public UserRoleService(IUserSessionManager userSessionManager)
        {
            _userSessionManager = userSessionManager;
        }

        [HttpPost]
        [Route("service/userRole/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, "UserRole", "gridView");
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
            var userInfo = _userSessionManager.GetUserInfo(dto.Ticket, Request);
            var bp = new BusinessParam(userInfo);
            var checkAccess = _userSessionManager.CheckAccess(bp, "UserRole",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? UserRoleFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/userRole/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, "UserRole", "update");
            return checkAccess.Done
                ? UserRoleFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/userRole/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, "UserRole", "delete");
            return checkAccess.Done
                ? UserRoleFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/userRole/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, "UserRole", "autocomplete");
            return checkAccess.Done
                ? UserRoleFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
    }
}