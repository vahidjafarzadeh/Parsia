using System.ComponentModel.DataAnnotations;

namespace Parsia.Core.UseCase
{
    public class UseCaseDto
    {
        [Display(Name = "شناسه")] public long EntityId { get; set; }
    }
}