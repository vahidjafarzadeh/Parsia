using System.Collections.Generic;
using DataLayer.Base;
using Parsia.Core.UseCaseAction;

namespace Parsia.Core.UseCase
{
    public class UseCaseDto : BaseDto
    {
        public long EntityId { get; set; }
        public string UseCaseName { get; set; }
        public string Clazz { get; set; }
        public string TableName { get; set; }
        public bool VirtualNode { get; set; }
        public UseCaseDto Parent { get; set; }
        public List<UseCaseActionDto> UseCaseActions { get; set; }

    }
}