using System.ComponentModel.DataAnnotations;
using DataLayer.Base;

namespace Parsia.Core.Action
{
    public class ActionDto : BaseDto
    {
        [Display(Name = "شناسه")] public long EntityId { get; set; }

        [Display(Name = "نام فعالیت")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string ActionName { get; set; }
        [Display(Name = "نام انگلیسی فعالیت")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string ActionEnName { get; set; }
    }
}