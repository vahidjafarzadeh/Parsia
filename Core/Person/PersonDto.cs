using System.ComponentModel.DataAnnotations;
using DataLayer.Base;
using Parsia.Core.ComboVal;
using Parsia.Core.File;
namespace Parsia.Core.Person
{
    public class PersonDto : BaseDto
    {
        [Display(Name = "شناسه")]
        public long EntityId { get; set; }
        [Display(Name = "کدملی")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(10, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string NationalCode { get; set; }
        [Display(Name = "شماره شناسنامه")]
        [MaxLength(10, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string PersianCode { get; set; }
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string LastName { get; set; }
        [Display(Name = "نام پدر")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string FatherName { get; set; }
        [Display(Name = "نام مادر")]
        [MaxLength(50, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string MotherName { get; set; }
        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        public long BirthDate { get; set; }
        [Display(Name = "محل تولد")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید")]
        [MaxLength(100, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string BirthPlace { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [Display(Name = "محل صدور شناسنامه")]
        [MaxLength(100, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string ExportationPlace { get; set; }
        [Display(Name = "شماره سری شناسنامه")]
        [MaxLength(10, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string CertificateSeries { get; set; }
        [Display(Name = "چپ دست")]
        public bool LeftHanded { get; set; }
        [Display(Name = "شماره موبایل")]
        [MaxLength(11, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string Mobile { get; set; }
        [Display(Name = "شماره تماس اضطراری")]
        [MaxLength(11, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string EmergencyPhone { get; set; }
        [Display(Name = "ایمیل")]
        [MaxLength(100, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "سریال شناسنامه")]
        [MaxLength(10, ErrorMessage = "حداکثر کاراکتر مجاز برای {0} به میزان {1} کاراکتر می باشد")]
        public string IdentitySerialNumber { get; set; }
        [Display(Name = "نمایه")]
        public FileDto File { get; set; }
        [Display(Name = "جنسیت")]
        public ComboValDto Sex { get; set; }
        [Display(Name = "وضعیت نظام وظیفه")]
        public ComboValDto MilitaryServiceStatus { get; set; }
        [Display(Name = "تحصیلات")]
        public ComboValDto Education { get; set; }
        [Display(Name = "گروه خونی")]
        public ComboValDto BloodType { get; set; }
        [Display(Name = "ملیت")]
        public ComboValDto Nationality { get; set; }
        [Display(Name = "وضعیت حیات")]
        public ComboValDto LifeStatus { get; set; }
        [Display(Name = "تابعیت")]
        public ComboValDto Citizenship { get; set; }
        [Display(Name = "دین")]
        public ComboValDto Religion { get; set; }
        [Display(Name = "مذهب")]
        public ComboValDto SubReligion { get; set; }
        [Display(Name = "وضعیت تاهل")]
        public ComboValDto MaritalStatus { get; set; }
        [Display(Name = "وضعیت مسکن")]
        public ComboValDto HousingSituation { get; set; }
        [Display(Name = "وضعیت جسمانی")]
        public ComboValDto HealthStatus { get; set; }
        [Display(Name = "نوع معلولیت")]
        public ComboValDto DisabilityType { get; set; }
    }
}