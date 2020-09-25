using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.Menu
{
    [ApiController]
    public class MenuService : ControllerBase
    {
        [HttpPost]
        [Route("service/menu/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Menu", "search");
            return checkAccess.Done
                ? MenuFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/menu/save")]
        public ServiceResult<object> Save(MenuDto dto)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Menu",
                dto.EntityId == null ? "edit" : "save");
            return checkAccess.Done ? MenuFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/menu/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Menu", "edit");
            return checkAccess.Done
                ? MenuFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/menu/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Menu", "delete");
            return checkAccess.Done
                ? MenuFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/menu/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Menu", "search");
            return checkAccess.Done
                ? MenuFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
    }
}