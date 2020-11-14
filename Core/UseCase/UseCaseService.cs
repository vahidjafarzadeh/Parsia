using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.UseCase
{
    [ApiController]
    public class UseCaseService : ControllerBase
    {
        [HttpPost]
        [Route("service/usecase/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UseCase", "gridView");
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
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UseCase",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? UseCaseFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/usecase/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UseCase", "update");
            return checkAccess.Done
                ? UseCaseFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/usecase/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UseCase", "delete");
            return checkAccess.Done
                ? UseCaseFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/usecase/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UseCase", "autocomplete");
            return checkAccess.Done
                ? UseCaseFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/usecase/getTotalUseCase/{getAllData}/{term}/{pageNumber}")]
        public ServiceResult<object> GetTotalUseCase(Clause clause,bool getAllData,string term,string pageNumber)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "UseCase", "gridView");
            return checkAccess.Done
                ? UseCaseFacade.GetInstance().GetTotalUseCase(bp,getAllData,term,pageNumber)
                : checkAccess;
        }
    }
}