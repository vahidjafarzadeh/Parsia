using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Base;
using Parsia.Core.AccessGroup;

namespace Parsia.Core.Role
{
    
    public class RoleDto : BaseDto
    {

        [Display(Name = "شناسه")]
        public long EntityId { get; set; }
        [Display(Name = "نقش")] 
        [MaxLength(10, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]  
        public string RoleName { get; set; }
        [Display(Name = "زمان سشن")]
        [Required] public long ExpireMinute { get; set; }
        public List<AccessGroupDto> AccessGroupDto { get; set; }

    }

}