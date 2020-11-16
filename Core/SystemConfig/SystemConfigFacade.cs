using System;
using DataLayer.Tools;
using Microsoft.AspNetCore.Http;

namespace Parsia.Core.SystemConfig
{
    public class SystemConfigFacade
    {
        private static readonly SystemConfigFacade Facade = new SystemConfigFacade();
        private static readonly SystemConfigCopier Copier = new SystemConfigCopier();
        private SystemConfigFacade()
        {
        }
        public static SystemConfigFacade GetInstance()
        {
            return Facade;
        }
        public ServiceResult<object> Save(BusinessParam bp, SystemConfigDto dto)
        {
            try
            {
                Copier.GetEntity(dto);
                return new ServiceResult<object>(dto, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "SystemConfigFacade.Save", bp.UserInfo);
            }
        }
        public ServiceResult<object> ShowRow(BusinessParam bp)
        {
            try
            {
                return new ServiceResult<object>(Copier.GetDto(), 1);

            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "SystemConfigFacade.ShowRow", bp.UserInfo);
            }
        }
        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new SystemConfigDto();
            if (!string.IsNullOrEmpty(request.Form["adminTitlePage"])) dto.AdminTitlePage = request.Form["adminTitlePage"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا عنوان پنل مدیریت را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["root"])) dto.Root = request.Form["root"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا مسیر فایل ها را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["apiHashEncryption"])) dto.ApiHashEncryption = request.Form["apiHashEncryption"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد هش را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["systemRoleId"])) dto.SystemRoleId = Convert.ToInt64(request.Form["systemRoleId"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه نقش سیستم را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["systemAdminRoleId"])) dto.SystemAdminRoleId = Convert.ToInt64(request.Form["systemAdminRoleId"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه نقش مدیر سیستم را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["applicationAdminRoleId"])) dto.ApplicationAdminRoleId = Convert.ToInt64(request.Form["applicationAdminRoleId"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه نقش مدیر اپلیکیشن را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["ticket"])) dto.Ticket = request.Form["ticket"];
            if (!string.IsNullOrEmpty(request.Form["adminValidIp"])) dto.AdminValidIp = request.Form["adminValidIp"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا ip معتبر برای ورود به پنل مدیریت را را وارد نمایید");
            return new ServiceResult<object>(dto, 1);
        }
    }
}
