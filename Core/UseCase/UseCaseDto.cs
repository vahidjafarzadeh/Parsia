using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Base;
using Parsia.Core.UseCaseAction;

namespace Parsia.Core.UseCase
{
    public class UseCaseDto : BaseDto
    {
        [Display(Name = "شناسه")] public long EntityId { get; set; }

        [Display(Name = "نام فرآیند")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string UseCaseName { get; set; }
        
        [Display(Name = "نام کلاس")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Clazz { get; set; }

        [Display(Name = "نام جدول")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string TableName { get; set; }

        [Display(Name = "والد")] public UseCaseDto Parent { get; set; }

        [Display(Name = "نود مجازی")] public bool VirtualNode { get; set; }
        [Display(Name = "اعمال")]
        public List<UseCaseActionDto> UseCaseActions { get; set; }

    }
}