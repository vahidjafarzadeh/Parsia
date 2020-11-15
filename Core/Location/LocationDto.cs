using System.ComponentModel.DataAnnotations;
using DataLayer.Base;
using Parsia.Core.ComboVal;

namespace Parsia.Core.Location
{
    public class LocationDto : BaseDto
    {
        [Display(Name = "شناسه")] public long EntityId { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Name { get; set; }

        [Display(Name = "والد")] public LocationDto Parent { get; set; }

        [Display(Name = "نوع تقسیمات کشوری")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")] 
        public ComboValDto Type { get; set; }
    }
}