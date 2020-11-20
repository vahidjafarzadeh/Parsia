using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Parsia.Core.Location
{
    [ApiController]
    [ClassDetails(Clazz = "Location", Facade = "LocationService")]
    public class LocationService : ControllerBase
    {
        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(LocationService).GetCustomAttributes(typeof(ClassDetails), true);

        private readonly IUserSessionManager _userSessionManager;

        public LocationService(IMemoryCache memoryCache, IUserSessionManager userSessionManager)
        {
            _userSessionManager = userSessionManager;
        }

        [HttpPost]
        [Route("service/location/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "gridView");
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
            var dto = (LocationDto) dtoFromRequest.Result;
            var userInfo = _userSessionManager.GetUserInfo(dto.Ticket, Request);
            var bp = new BusinessParam(userInfo);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz,
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? LocationFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/location/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "update");
            return checkAccess.Done
                ? LocationFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/location/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "delete");
            return checkAccess.Done
                ? LocationFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/location/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "autocomplete");
            return checkAccess.Done
                ? LocationFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
    }
}