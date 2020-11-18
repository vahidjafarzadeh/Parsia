using DataLayer.Base;

namespace Parsia.Core.Action
{
    public class ActionDto : BaseDto
    {
        public long EntityId { get; set; }
        public string ActionName { get; set; }
        public string ActionEnName { get; set; }
    }
}