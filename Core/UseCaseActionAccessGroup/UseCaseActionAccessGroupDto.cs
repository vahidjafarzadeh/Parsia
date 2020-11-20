using DataLayer.Base;
using Parsia.Core.AccessGroup;
using Parsia.Core.UseCaseAction;

namespace Parsia.Core.UseCaseActionAccessGroup
{
    public class UseCaseActionAccessGroupDto : BaseDto
    {
        public long EntityId { get; set; }
        public UseCaseActionDto UseCaseAction { get; set; }
        public AccessGroupDto AccessGroup { get; set; }
    }
}