using System.Threading.Tasks;
using DataLayer.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

ing Microsoft.Extensions.Caching.Memory;

namespace Parsia.Core.File
{
    [ApiController]
    [ClassDetails(Clazz = "File", Facade = "FileService")]
    public class FileService : ControllerBase
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private static readonly ClassDetails[] ClassDetails = (ClassDetails[])typeof(FileService).GetCustomAttributes(typeof(ClassDetails), true);
        private readonly IUserSessionManager _userSessionManager;
        public FileService(IWebHostEnvironment environment, IUserSessionManager userSessionManager)
        {
            _hostEnvironment = environment;
            _userSessionManager = userSessionManager;
        }
        
        [HttpPost]
        [Route("service/file/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "gridView");
            return checkAccess.Done
                ? FileFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/save")]
        public ServiceResult<object> Save(FileDto dto)
        {
            if (!ModelState.IsValid)
               Enumerator.ErrorCodeesult<object>(Enumerator.ErrorCode.MoEnumerator.ErrorCode              Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = _userSessionManager.GetUserInfo(dto.Ticket, Request);
            var bp = new BusinessParam(userInfo);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? FileFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/file/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiEnumerator.ErrorCodemerator.ErrorCode.ModelNotValid,
   Enumerator.ErrorCodemerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "update");
            return checkAccess.Done
                ? FileFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "delete");
            return checkAccess.Done
                ? FileFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiEnumerator.ErrorCodemerator.ErrorCode.ModelNotValid,
   Enumerator.ErrorCodemerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "autocomplete");
            return checkAccess.Done
                ? FileFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }

        //other need
        [HttpPost]
        [Route("service/file/getAllExtension")]
        public ServiceResult<object> GetAllExtension(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "gridView");
            return checkAccess.Done
                ? FileFacade.GetInstance().GetAllExtension(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/createFolder")]
        public ServiceResult<object> CreateFolder(FolderDto dto)
        {
            var userInfo = _userSessionManager.GetUserInfo(dto.Ticket, Request);
            var bp = new BusinessParam(userInfo, _hostEnvironment);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "insert");
            return checkAccess.Done
                ? FileFacade.GetInstance().CreateFolder(bp, dto)
                : checkAccess;
        }


        [HttpPost]
        [Route("service/file/createFile")]
        public ServiceResult<object> CreateFile()
        {
            var ticket = Request.Form["ticket"];
            var userInfo = _userSessionManager.GetUserInfo(ticket, Request);
            var bp = new BusinessParam(userInfo, _hostEnvironment);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "insert");
            return checkAccess.Done
                ? FileFacade.GetInstance().CreateFile(bp, Request).Result
                : checkAccess;
        }

        [HttpPost]
        [Route("service/file/getDetails")]
        public ServiceResult<object> GetDetails(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "gridView");
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
            return await FileFacade.GetInstance().Download(file, bp, thumb
    
        }
    }
}