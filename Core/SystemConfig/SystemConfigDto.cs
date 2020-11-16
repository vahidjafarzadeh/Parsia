using DataLayer.Base;

namespace Parsia.Core.SystemConfig
{
    public class SystemConfigDto:BaseDto
    {
        public string AdminTitlePage { get; set; }
        public string Root { get; set; }
        public string ApiHashEncryption { get; set; }
        public long SystemRoleId { get; set; }
        public long SystemAdminRoleId { get; set; }
        public long ApplicationAdminRoleId { get; set; }
        public string AdminValidIp { get; set; }
    }
}
