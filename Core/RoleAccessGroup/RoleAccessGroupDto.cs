using DataLayer.Base;
using Parsia.Core.AccessGroup;
using Parsia.Core.Role;

namespace Parsia.Core.RoleAccessGroup
{
    public class RoleAccessGroupDto : BaseDto
    {
        public long EntityId { get; set; }
        public RoleDto Role { get; set; }
        public AccessGroupDto AccessGroup { get; set; }
    }
}
