using System.ComponentModel.DataAnnotations;
using DataLayer.Base;
using Parsia.Core.AccessGroup;
using Parsia.Core.UseCaseAction;

namespace Parsia.Core.UseCaseActionAccessGroup
{
    public class UseCaseActionAccessGroupDto:BaseDto
    {
        [Display(Name = "شناسه")] public long EntityId { get; set; }

        [Display(Name = "عملیات")] public UseCaseActionDto UseCaseAction { get; set; }
        [Display(Name = "گروه دسترسی")] public AccessGroupDto AccessGroup { get; set; }
    }
}
