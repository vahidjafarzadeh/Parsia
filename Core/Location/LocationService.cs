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
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Location", "gridView");
            return checkAccess.Done
                ? LocationFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/location/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = LocationFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (LocationDto)dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Location",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? LocationFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/location/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Location", "update");
            return checkAccess.Done
                ? LocationFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/location/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
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
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Location", "autocomplete");
            return checkAccess.Done
                ? LocationFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
    }
}