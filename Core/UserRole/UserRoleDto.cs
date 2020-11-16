using DataLayer.Base;
using Parsia.Core.Organization;
using Parsia.Core.Role;

namespace Parsia.Core.UserRole
{

    public class UserRoleDto : BaseDto
    {

        public long EntityId { get; set; }

        public string OrgAccess { get; set; }

        public OrganizationDto Organization { get; set; }

        public RoleDto Role { get; set; }

        public UserDto User { get; set; }

    }

}