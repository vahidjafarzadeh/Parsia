using System.ComponentModel.DataAnnotations;
using DataLayer.Base;
using Parsia.Core.ComboVal;
using Parsia.Core.File;
using Parsia.Core.UseCase;

namespace Parsia.Core.Menu
{
    public class MenuDto:BaseDto
    {
        [Display(Name = "شناسه")]
        public long EntityId { get; set; }
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(100, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Name { get; set; }
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(100, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Title { get; set; }
        [Display(Name = "مسیر")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(300, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        [StringLength(300)] public string Path { get; set; }
        [Display(Name = "آیکون")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(300, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Icon { get; set; }
        [Display(Name = "اولویت")]
        public int OrderNode { get; set; }
        [Display(Name = "تصویر")]
        public FileDto File { get; set; }
        [Display(Name = "دسترسی")]
        public UseCaseDto UseCase { get; set; }
        [Display(Name = "والد")]
        public long? ParentId { get; set; }
        [Display(Name = "نحوه نمایش")]
        public ComboValDto Target { get; set; }
    }
}
