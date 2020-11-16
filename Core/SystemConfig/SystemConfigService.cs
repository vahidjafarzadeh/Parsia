using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.SystemConfig
{
    [ApiController]
    public class SystemConfigService : ControllerBase
    {

        [HttpPost]
        [Route("service/systemConfig/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = SystemConfigFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (SystemConfigDto)dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "SystemConfig", "update");
            return checkAccess.Done ? SystemConfigFacade.GetInstance().Save(bp, dto) : checkAccess;
        }
        [HttpPost]
        [Route("service/systemConfig/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "SystemConfig", "update");
            return checkAccess.Done
                ? SystemConfigFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }
    }
}
