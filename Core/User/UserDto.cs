using System;
using System.ComponentModel.DataAnnotations;
using DataLayer.Base;

namespace Parsia.Core.User
{
    public class UserDto : BaseDto
    {
        [Display(Name = "شناسه کاربر")] public long EntityId { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(100, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Username { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "کد فعال سازی ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string EmailCode { get; set; }

        [Display(Name = "کد فعال سازی موبایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(6, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string PhoneCode { get; set; }

        [Display(Name = "تعداد تلاش ناموفق")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        public short Attempt { get; set; }

        [Display(Name = "آخرین بازدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [DataType(DataType.DateTime)]
        public DateTime LastVisit { get; set; }
    }
}