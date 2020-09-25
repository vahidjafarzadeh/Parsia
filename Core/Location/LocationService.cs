using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.Location
{
    [ApiController]
    public class LocationService : ControllerBase
    {
        [HttpPost]
        [Route("service/location/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Location", "search");
            return checkAccess.Done
                ? LocationFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/location/save")]
        public ServiceResult<object> Save(LocationDto dto)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Location",
                dto.EntityId == null ? "edit" : "save");
            return checkAccess.Done ? LocationFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/location/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Location", "edit");
            return checkAccess.Done
                ? LocationFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/location/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Location", "delete");
            return checkAccess.Done
                ? LocationFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/location/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            if (!ModelState.IsValid)
                return new ServiceResult<object>(Enumerator.ErrorCode.ModelNotValid,
                    Enumerator.ErrorCode.ModelNotValid.GetDescription());
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Location", "search");
            return checkAccess.Done
                ? LocationFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
    }
}
