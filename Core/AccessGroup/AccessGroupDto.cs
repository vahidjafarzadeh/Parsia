using System.Collections.Generic;
using DataLayer.Base;
using Parsia.Core.UseCaseAction;

namespace Parsia.Core.AccessGroup
{
    public class AccessGroupDto : BaseDto
    {
        public long EntityId { get; set; }
        public string Name { get; set; }
        public List<UseCaseActionDto> UseCaseActionList { get; set; }
    }
}