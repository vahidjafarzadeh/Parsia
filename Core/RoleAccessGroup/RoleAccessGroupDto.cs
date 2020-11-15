using System.ComponentModel.DataAnnotations;
using DataLayer.Base;
using Parsia.Core.AccessGroup;
using Parsia.Core.Role;

namespace Parsia.Core.RoleAccessGroup
{
    public class RoleAccessGroupDto : BaseDto
    {
        [Display(Name = "شناسه")] public long EntityId { get; set; }

        [Display(Name = "نقش")] public RoleDto Role { get; set; }
        [Display(Name = "گروه دسترسی")] public AccessGroupDto AccessGroup { get; set; }
    }
}
