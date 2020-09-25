using System.ComponentModel.DataAnnotations;
using DataLayer.Base;

namespace Parsia.Core.AccessGroup
{
    public class AccessGroupDto : BaseDto
    {
        [Display(Name = "شناسه")] public long EntityId { get; set; }

        [Display(Name = "نام گروه دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]

        public string Name { get; set; }
    }
}