using System;
using System.Diagnostics;
using DataLayer.Tools;
using Microsoft.AspNetCore.Http;

namespace Parsia.Core.SystemConfig
{
    [ClassDetails(Clazz = "SystemConfig", Facade = "SystemConfigFacade")]
    public class SystemConfigFacade
    {
        private static readonly SystemConfigFacade Facade = new SystemConfigFacade();
        private static readonly SystemConfigCopier Copier = new SystemConfigCopier();
        private static readonly ClassDetails[] ClassDetails = (ClassDetails[])typeof(SystemConfigFacade).GetCustomAttributes(typeof(ClassDetails), true);

        private SystemConfigFacade()
        {
        }
        public static SystemConfigFacade GetInstance()
        {
            return Facade;
        }
        public ServiceResult<object> Save(BusinessParam bp, SystemConfigDto dto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                Copier.GetEntity(dto);
                return new ServiceResult<object>(dto, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }
        public ServiceResult<object> ShowRow(BusinessParam bp)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                return new ServiceResult<object>(Copier.GetDto(), 1);

            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }
        public ServiceResult<object> GetDtoFromRequest(HttpRequest request)
        {
            var dto = new SystemConfigDto();
            if (!string.IsNullOrEmpty(request.Form["adminTitlePage"])) dto.AdminTitlePage = request.Form["adminTitlePage"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا عنوان پنل مدیریت را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["root"])) dto.Root = request.Form["root"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا مسیر فایل ها را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["apiHashEncryption"])) dto.ApiHashEncryption = request.Form["apiHashEncryption"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد هش را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["systemRoleId"])) dto.SystemRoleId = Convert.ToInt64(request.Form["systemRoleId"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه نقش سیستم را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["userRoleId"])) dto.UserRoleId = Convert.ToInt64(request.Form["userRoleId"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه نقش کاربر سایت را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["ticket"])) dto.Ticket = request.Form["ticket"];
            if (!string.IsNullOrEmpty(request.Form["adminValidIp"])) dto.AdminValidIp = request.Form["adminValidIp"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا ip معتبر برای ورود به پنل مدیریت را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["applicationUrl"])) dto.ApplicationUrl = request.Form["applicationUrl"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا آدرس سایت را وارد نمایید");
            return new ServiceResult<object>(dto, 1);
        }
    }
}
