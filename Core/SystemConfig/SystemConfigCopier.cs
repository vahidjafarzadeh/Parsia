namespace Parsia.Core.SystemConfig
{
    public class SystemConfigCopier
    {
        public SystemConfigDto GetDto()
        {
            var to = new SystemConfigDto
            {
                AdminTitlePage = DataLayer.Tools.SystemConfig.AdminTitlePage,
                AdminValidIp = DataLayer.Tools.SystemConfig.AdminValidIp,
                ApiHashEncryption = DataLayer.Tools.SystemConfig.ApiHashEncryption,
                UserRoleId = DataLayer.Tools.SystemConfig.UserRoleId,
                Root = DataLayer.Tools.SystemConfig.Root,
                SystemRoleId = DataLayer.Tools.SystemConfig.SystemRoleId,
                ApplicationUrl = DataLayer.Tools.SystemConfig.ApplicationUrl,
                DevelopMode = DataLayer.Tools.SystemConfig.DevelopMode,
                EmailHost = DataLayer.Tools.SystemConfig.EmailHost,
                EmailPasswordHost = DataLayer.Tools.SystemConfig.EmailPasswordHost,
                EmailPortSmtpHost = DataLayer.Tools.SystemConfig.EmailPortSmtpHost,
                EmailSiteName = DataLayer.Tools.SystemConfig.EmailSiteName,
                EmailSmtpHost = DataLayer.Tools.SystemConfig.EmailSmtpHost,
                ValidUrls = DataLayer.Tools.SystemConfig.ValidUrls,
                TemplateIdVerificationCode = DataLayer.Tools.SystemConfig.TemplateIdVerificationCode,
                MaxAttemptLogin = DataLayer.Tools.SystemConfig.MaxAttemptLogin,
                MenuCacheTimeMinute = DataLayer.Tools.SystemConfig.MenuCacheTimeMinute,
                SmsSecretKey = DataLayer.Tools.SystemConfig.SmsSecretKey,
                SmsUserApiKey = DataLayer.Tools.SystemConfig.SmsUserApiKey,
                TemplateIdAdminFactorCode = DataLayer.Tools.SystemConfig.TemplateIdAdminFactorCode,
                TemplateIdBlockIpCode = DataLayer.Tools.SystemConfig.TemplateIdBlockIpCode,
                IpBlackList = DataLayer.Tools.SystemConfig.IpBlackList,
                TemplateIdRecoveryPasswordCode = DataLayer.Tools.SystemConfig.TemplateIdRecoveryPasswordCode,
                TemplateIdRememberMeCode = DataLayer.Tools.SystemConfig.TemplateIdRememberMeCode,
                TemplateIdUserFactorCode = DataLayer.Tools.SystemConfig.TemplateIdUserFactorCode
            };

            return to;
        }

        public void GetEntity(SystemConfigDto dto)
        {
            DataLayer.Tools.SystemConfig.AdminTitlePage = dto.AdminTitlePage;
            DataLayer.Tools.SystemConfig.AdminValidIp = dto.AdminValidIp;
            DataLayer.Tools.SystemConfig.ApiHashEncryption = dto.ApiHashEncryption;
            DataLayer.Tools.SystemConfig.UserRoleId = dto.UserRoleId;
            DataLayer.Tools.SystemConfig.Root = dto.Root;
            DataLayer.Tools.SystemConfig.SystemRoleId = dto.SystemRoleId;
            DataLayer.Tools.SystemConfig.ApplicationUrl = dto.ApplicationUrl;
            DataLayer.Tools.SystemConfig.DevelopMode = dto.DevelopMode;
            DataLayer.Tools.SystemConfig.EmailHost = dto.EmailHost;
            DataLayer.Tools.SystemConfig.EmailPasswordHost = dto.EmailPasswordHost;
            DataLayer.Tools.SystemConfig.EmailPortSmtpHost = dto.EmailPortSmtpHost;
            DataLayer.Tools.SystemConfig.EmailSiteName = dto.EmailSiteName;
            DataLayer.Tools.SystemConfig.EmailSmtpHost = dto.EmailSmtpHost;
            DataLayer.Tools.SystemConfig.ValidUrls = dto.ValidUrls;
            DataLayer.Tools.SystemConfig.TemplateIdVerificationCode = dto.TemplateIdVerificationCode;
            DataLayer.Tools.SystemConfig.MaxAttemptLogin = dto.MaxAttemptLogin;
            DataLayer.Tools.SystemConfig.MenuCacheTimeMinute = dto.MenuCacheTimeMinute;
            DataLayer.Tools.SystemConfig.SmsSecretKey = dto.SmsSecretKey;
            DataLayer.Tools.SystemConfig.SmsUserApiKey = dto.SmsUserApiKey;
            DataLayer.Tools.SystemConfig.TemplateIdAdminFactorCode = dto.TemplateIdAdminFactorCode;
            DataLayer.Tools.SystemConfig.TemplateIdBlockIpCode = dto.TemplateIdBlockIpCode;
            DataLayer.Tools.SystemConfig.IpBlackList = dto.IpBlackList;
            DataLayer.Tools.SystemConfig.TemplateIdRecoveryPasswordCode = dto.TemplateIdRecoveryPasswordCode;
            DataLayer.Tools.SystemConfig.TemplateIdRememberMeCode = dto.TemplateIdRememberMeCode;
            DataLayer.Tools.SystemConfig.TemplateIdUserFactorCode = dto.TemplateIdUserFactorCode;
        }
    }
}