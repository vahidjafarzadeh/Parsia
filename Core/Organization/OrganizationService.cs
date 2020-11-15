﻿using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.Organization
{
    [ApiController]
    public class OrganizationService : ControllerBase
    {
        [HttpPost]
        [Route("service/organization/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Organization", "gridView");
            return checkAccess.Done
                ? OrganizationFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/organization/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = OrganizationFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (OrganizationDto)dtoFromRequest.Result;
            var userInfo = UserSessionManager.GetUserInfo(dto.Ticket);
            var bp = new BusinessParam(userInfo);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Organization",
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? OrganizationFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/organization/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Organization", "update");
            return checkAccess.Done
                ? OrganizationFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/organization/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Organization", "delete");
            return checkAccess.Done
                ? OrganizationFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/organization/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = UserSessionManager.GetUserInfo(clause.Ticket);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = UserSessionManager.CheckAccess(bp, "Organization", "autocomplete");
            return checkAccess.Done
                ? OrganizationFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }
    

       
    }
}