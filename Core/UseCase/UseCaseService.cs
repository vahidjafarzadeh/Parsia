using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.UseCase
{
    [ApiController]
    [ClassDetails(Clazz = "UseCase", Facade = "UseCaseService")]
    public class UseCaseService : ControllerBase
    {
        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(UseCaseService).GetCustomAttributes(typeof(ClassDetails), true);

        private readonly IUserSessionManager _userSessionManager;

        public UseCaseService(IUserSessionManager userSessionManager)
        {
            _userSessionManager = userSessionManager;
        }

        [HttpPost]
        [Route("service/usecase/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "gridView");
            return checkAccess.Done
                ? UseCaseFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/usecase/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = UseCaseFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (UseCaseDto) dtoFromRequest.Result;
            var userInfo = _userSessionManager.GetUserInfo(dto.Ticket, Request);
            var bp = new BusinessParam(userInfo);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz,
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? UseCaseFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/usecase/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "update");
            return checkAccess.Done
                ? UseCaseFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/usecase/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "delete");
            return checkAccess.Done
                ? UseCaseFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/usecase/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "autocomplete");
            return checkAccess.Done
                ? UseCaseFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/usecase/getTotalUseCase/{getAllData}/{term}/{pageNumber}")]
        public ServiceResult<object> GetTotalUseCase(Clause clause, bool getAllData, string term, string pageNumber)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "gridView");
            return checkAccess.Done
                ? UseCaseFacade.GetInstance().GetTotalUseCase(bp, getAllData, term, pageNumber)
                : checkAccess;
        }
    }
}