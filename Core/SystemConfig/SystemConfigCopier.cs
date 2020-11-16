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
                ApplicationAdminRoleId = DataLayer.Tools.SystemConfig.ApplicationAdminRoleId,
                Root = DataLayer.Tools.SystemConfig.Root,
                SystemAdminRoleId = DataLayer.Tools.SystemConfig.SystemAdminRoleId,
                SystemRoleId = DataLayer.Tools.SystemConfig.SystemRoleId,
            };

            return to;
        }
        public void GetEntity(SystemConfigDto dto)
        {
             DataLayer.Tools.SystemConfig.AdminTitlePage = dto.AdminTitlePage;
             DataLayer.Tools.SystemConfig.AdminValidIp = dto.AdminValidIp;
             DataLayer.Tools.SystemConfig.ApiHashEncryption = dto.ApiHashEncryption;
             DataLayer.Tools.SystemConfig.ApplicationAdminRoleId = dto.ApplicationAdminRoleId;
             DataLayer.Tools.SystemConfig.Root = dto.Root;
             DataLayer.Tools.SystemConfig.SystemAdminRoleId = dto.SystemAdminRoleId;
             DataLayer.Tools.SystemConfig.SystemRoleId = dto.SystemRoleId;
        }
    }
}
