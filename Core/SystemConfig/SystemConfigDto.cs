using DataLayer.Base;

namespace Parsia.Core.SystemConfig
{
    public class SystemConfigDto : BaseDto
    {
        public string AdminTitlePage { get; set; }
        public string Root { get; set; }
        public string ApiHashEncryption { get; set; }
        public long SystemRoleId { get; set; }
        public long UserRoleId { get; set; }
        public string AdminValidIp { get; set; }
        public string ApplicationUrl { get; set; }

        public double MenuCacheTimeMinute { get; set; }
        public string EmailSmtpHost { get; set; }
        public string EmailHost { get; set; }
        public string EmailPortSmtpHost { get; set; }
        public string EmailPasswordHost { get; set; }
        public string EmailSiteName { get; set; }
        public string SmsUserApiKey { get; set; }
        public string SmsSecretKey { get; set; }
        public string TemplateIdVerificationCode { get; set; }
        public string TemplateIdRecoveryPasswordCode { get; set; }
        public string TemplateIdRememberMeCode { get; set; }
        public string TemplateIdUserFactorCode { get; set; }
        public string TemplateIdAdminFactorCode { get; set; }
        public string TemplateIdBlockIpCode { get; set; }
        public long MaxAttemptLogin { get; set; }
        public bool DevelopMode { get; set; }
        public string ValidUrls { get; set; }
        public string IpBlackList { get; set; }
    }
}