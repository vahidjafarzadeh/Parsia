using DataLayer.Base;
using Parsia.Core.Organization;
using Parsia.Core.Role;

namespace Parsia.Core.EntityState
{
    public class EntityStateDto : BaseDto
    {
        public long EntityId { get; set; }
        public string TableName { get; set; }
        public long TableEntityId { get; set; }
        public OrganizationDto Organization { get; set; }
        public RoleDto Role { get; set; }
        public UserDto User { get; set; }
    }
}