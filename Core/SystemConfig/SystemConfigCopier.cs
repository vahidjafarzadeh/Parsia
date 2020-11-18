namespace Parsia.Core.SystemConfig
{
    public class SystemConfigCopier
    {
        public SystemConfigDto GetDto()
        {
            var to = new SystemConfigDto()
            {
                AdminTitlePage = DataLayer.Tools.SystemConfig.AdminTitlePage,
                AdminValidIp = DataLayer.Tools.SystemConfig.AdminValidIp,
                ApiHashEncryption = DataLayer.Tools.SystemConfig.ApiHashEncryption,
                UserRoleId = DataLayer.Tools.SystemConfig.UserRoleId,
                Root = DataLayer.Tools.SystemConfig.Root,
                SystemRoleId = DataLayer.Tools.SystemConfig.SystemRoleId,
                ApplicationUrl = DataLayer.Tools.SystemConfig.ApplicationUrl,
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
        }
    }
}
