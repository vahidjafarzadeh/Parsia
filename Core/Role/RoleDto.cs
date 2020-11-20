using System.Collections.Generic;
using DataLayer.Base;
using Parsia.Core.AccessGroup;

namespace Parsia.Core.Role
{
    public class RoleDto : BaseDto
    {
        public long EntityId { get; set; }
        public string RoleName { get; set; }
        public long ExpireMinute { get; set; }
        public List<AccessGroupDto> AccessGroupDto { get; set; }
    }
}