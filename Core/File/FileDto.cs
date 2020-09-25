using System.ComponentModel.DataAnnotations;
using DataLayer.Base;

namespace Parsia.Core.File
{
    public class FileDto : BaseDto
    {
        [Display(Name = "شناسه")] public long EntityId { get; set; }

        [Display(Name = "نام فایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Name { get; set; }

        [Display(Name = "مسیر")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Path { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Title { get; set; }

        [Display(Name = "صفت جایگزین")]
        [MaxLength(100, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Alt { get; set; }

        [Display(Name = "حجم")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string DataSize { get; set; }

        [Display(Name = "نوع دیتا")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public static string DataType { get; set; }

        [Display(Name = "توضیحات")] public string Description { get; set; }

        [Display(Name = "پسوند")]
        [MaxLength(10, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Extension { get; set; }

        [Display(Name = "والد")] public long? ParentId { get; set; }

        [Display(Name = "عکس کوچک شده")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Thumbnail { get; set; }

        [Display(Name = "نمایش در مدیریت فایل")]
        public bool DisplayInFileManager { get; set; }
    }
}