using DataLayer.Base;
using Parsia.Core.Action;
using Parsia.Core.UseCase;

namespace Parsia.Core.UseCaseAction
{
    public class UseCaseActionDto : BaseDto
    {
        public long EntityId { get; set; }
        public ActionDto Action { get; set; }
        public UseCaseDto UseCase { get; set; }
    }
}