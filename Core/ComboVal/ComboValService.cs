using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.ComboVal
{
    [ApiController]
    public class ComboValService : ControllerBase
    {
        [HttpPost]
        [Route("service/comboVal/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "ComboVal", "gridView");
            return checkAccess.Done
                ? ComboValFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/comboVal/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = ComboValFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (ComboValDto) dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "ComboVal",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? ComboValFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/comboVal/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "ComboVal", "update");
            return checkAccess.Done
                ? ComboValFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/comboVal/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "ComboVal", "delete");
            return checkAccess.Done
                ? ComboValFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/comboVal/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "ComboVal", "autocomplete");
            return checkAccess.Done
                ? ComboValFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
        [HttpPost]
        [Route("service/comboVal/autocompleteView/parent")]
        public ServiceResult<object> AutocompleteViewParent(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "ComboVal", "autocomplete");
            return checkAccess.Done
                ? ComboValFacade.GetInstance().AutocompleteViewParent(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/comboVal/getAccess")]
        public ServiceResult<bool> GetAccess(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            return SystemConfig.IsUnlimitedRole(userInfo.RoleId, false)
                ? new ServiceResult<bool>(true, 1)
                : new ServiceResult<bool>(false, 1);
        }
    }
}