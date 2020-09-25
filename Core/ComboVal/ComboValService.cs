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
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "ComboVal", "search");
            return checkAccess.Done
                ? ComboValFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/comboVal/save")]
        public ServiceResult<object> Save(ComboValDto dto)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "ComboVal",
                dto.EntityId == null ? "edit" : "save");
            return checkAccess.Done ? ComboValFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/comboVal/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "ComboVal", "edit");
            return checkAccess.Done
                ? ComboValFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/comboVal/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
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
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "ComboVal", "search");
            return checkAccess.Done
                ? ComboValFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
    }
}
