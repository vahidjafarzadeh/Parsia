using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.File
{
    [ApiController]
    public class FileService : ControllerBase
    {
        [HttpPost]
        [Route("service/file/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "search");
            return checkAccess.Done
                ? FileFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/save")]
        public ServiceResult<object> Save(FileDto dto)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "File",
                dto.EntityId == null ? "edit" : "save");
            return checkAccess.Done ? FileFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/file/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "edit");
            return checkAccess.Done
                ? FileFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "delete");
            return checkAccess.Done
                ? FileFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "search");
            return checkAccess.Done
                ? FileFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
    }
}
