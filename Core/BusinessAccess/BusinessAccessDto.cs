using DataLayer.Base;
using Parsia.Core.Organization;
using Parsia.Core.Role;

namespace Parsia.Core.BusinessAccess
{
    public class BusinessAccessDto : BaseDto
    {
        public long EntityId { get; set; }
        public string UseCase { get; set; }
        public string EntityIds { get; set; }
        public bool AddCurrentList { get; set; }
        public OrganizationDto Organization { get; set; }
        public RoleDto Role { get; set; }
    }
}