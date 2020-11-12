using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.Person
{
    [ApiController]
    public class PersonService : ControllerBase
    {
        [HttpPost]
        [Route("service/person/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Person", "search");
            return checkAccess.Done
                ? PersonFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/person/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = PersonFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (PersonDto) dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Person",
                dto.EntityId == null ? "edit" : "save");
            return checkAccess.Done ? PersonFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/person/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Person", "edit");
            return checkAccess.Done
                ? PersonFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/person/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Person", "delete");
            return checkAccess.Done
                ? PersonFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/person/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Person", "search");
            return checkAccess.Done
                ? PersonFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }


        [HttpPost]
        [Route("service/person/getAccess")]
        public ServiceResult<bool> GetAccess(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            return SystemConfig.IsUnlimitedRole(userInfo.RoleId, false)
                ? new ServiceResult<bool>(true, 1)
                : new ServiceResult<bool>(false, 1);
        }
    }
}