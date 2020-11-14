using System.Threading.Tasks;
using DataLayer.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.File
{
    [ApiController]
    public class FileService : ControllerBase
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public FileService(IWebHostEnvironment environment)
        {
            _hostEnvironment = environment;
        }

        [HttpPost]
        [Route("service/file/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "gridView");
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
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", dto.EntityId == 0 ? "insert" : "update");
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
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "update");
            return checkAccess.Done
                ? FileFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
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
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "autocomplete");
            return checkAccess.Done
                ? FileFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }

        //other need
        [HttpPost]
        [Route("service/file/getAllExtension")]
        public ServiceResult<object> GetAllExtension(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "gridView");
            return checkAccess.Done
                ? FileFacade.GetInstance().GetAllExtension(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/createFolder")]
        public ServiceResult<object> CreateFolder(FolderDto dto)
        {
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo, _hostEnvironment);
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "insert");
            return checkAccess.Done
                ? FileFacade.GetInstance().CreateFolder(bp, dto)
                : checkAccess;
        }


        [HttpPost]
        [Route("service/file/createFile")]
        public ServiceResult<object> CreateFile()
        {
            var ticket = Request.Form["ticket"];
            var userInfo = UserSessionManager.GetUserInfo(ticket);
            var bp = new BusinessParam(userInfo, _hostEnvironment);
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "insert");
            return checkAccess.Done
                ? FileFacade.GetInstance().CreateFile(bp, Request).Result
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/getDetails")]
        public ServiceResult<object> GetDetails(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "File", "gridView");
            return checkAccess.Done
                ? FileFacade.GetInstance().GetDetails(bp)
                : checkAccess;
        }

        [HttpGet]
        [Route("service/file/download/{file}")]
        public async Task<IActionResult> Download(string file)
        {
            var thumbnail = Request.Query.ContainsKey("thumbnail");
            var bp = new BusinessParam(_hostEnvironment);
            return await FileFacade.GetInstance().Download(file, bp, thumbnail);
        }
    }
}