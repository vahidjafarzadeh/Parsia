using System.ComponentModel.DataAnnotations;
using DataLayer.Base;

namespace Parsia.Core.ComboVal
{
    public class ComboValDto : BaseDto
    {
        [Display(Name = "شناسه")] public long EntityId { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(100, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Name { get; set; }

        [Display(Name = "مقدار")]
        [MaxLength(100, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Value { get; set; }

        [Display(Name = "والد")] public ComboValDto Parent { get; set; }

        [Display(Name = "نمایش به مدیر")] public bool AdminOnly { get; set; }
    }
}