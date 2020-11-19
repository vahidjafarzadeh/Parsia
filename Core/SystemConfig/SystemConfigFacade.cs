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
            if (!string.IsNullOrEmpty(request.Form["menuCacheTimeMinute"])) dto.MenuCacheTimeMinute = Convert.ToDouble(request.Form["menuCacheTimeMinute"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا زمان کش منوهای سایت را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["emailSmtpHost"])) dto.EmailSmtpHost = request.Form["emailSmtpHost"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا smtp ایمیل را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["emailHost"])) dto.EmailHost = request.Form["emailHost"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا آدرس ایمیل را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["emailPortSmtpHost"])) dto.EmailPortSmtpHost = request.Form["emailPortSmtpHost"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا پورت smtp ایمیل را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["emailPasswordHost"])) dto.EmailPasswordHost = request.Form["emailPasswordHost"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا پسورد ایمیل را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["emailSiteName"])) dto.EmailSiteName = request.Form["emailSiteName"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام سایت درون ایمیل های ارسالی را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["smsUserApiKey"])) dto.SmsUserApiKey = request.Form["smsUserApiKey"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا api key اس ام اس را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["smsSecretKey"])) dto.SmsSecretKey = request.Form["smsSecretKey"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا security key اس ام اس  را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["templateIdVerificationCode"])) dto.TemplateIdVerificationCode = request.Form["templateIdVerificationCode"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه قالب اس ام اس کد فعال سازی را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["templateIdRecoveryPasswordCode"])) dto.TemplateIdRecoveryPasswordCode = request.Form["templateIdRecoveryPasswordCode"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه قالب اس ام اس کد بازیابی کلمه عبور را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["templateIdRememberMeCode"])) dto.TemplateIdRememberMeCode = request.Form["templateIdRememberMeCode"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه قالب اس ام اس کد اطلاع رسانی را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["templateIdUserFactorCode"])) dto.TemplateIdUserFactorCode = request.Form["templateIdUserFactorCode"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه قالب اس ام اس کد فاکتور برای مشتری را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["templateIdAdminFactorCode"])) dto.TemplateIdAdminFactorCode = request.Form["templateIdAdminFactorCode"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه قالب اس ام اس کد فاکتور برای مدیران را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["templateIdBlockIpCode"])) dto.TemplateIdBlockIpCode = request.Form["templateIdBlockIpCode"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا شناسه قالب اس ام اس کد کاربران مسدود شده را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["maxAttemptLogin"])) dto.MaxAttemptLogin = Convert.ToInt64(request.Form["maxAttemptLogin"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا حداکثر تعداد تلاش ناموفق برای ورود به سیستم را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["developMode"])) dto.DevelopMode = Convert.ToBoolean(request.Form["developMode"]); else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا سایت در حال به روزرسانی می باشد را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["validUrls"])) dto.ValidUrls = request.Form["validUrls"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا آدرس معتبر هنگام به روزرسانی سایت را وارد نمایید");
            if (!string.IsNullOrEmpty(request.Form["ipBlackList"])) dto.IpBlackList = request.Form["ipBlackList"]; else return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا ip مسدود شده را وارد نمایید");
            return new ServiceResult<object>(dto, 1);
        }
    }
}
