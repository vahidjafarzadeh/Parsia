using System.ComponentModel.DataAnnotations;
using DataLayer.Base;
using Parsia.Core.Action;
using Parsia.Core.UseCase;

namespace Parsia.Core.UseCaseAction
{
    public class UseCaseActionDto:BaseDto
    {
        [Display(Name = "شناسه")]  public long EntityId { get; set; }
        [Display(Name = "عملیات")] public ActionDto Action { get; set; }
        [Display(Name = "فرآیند")] public UseCaseDto UseCase { get; set; }
    }
}
