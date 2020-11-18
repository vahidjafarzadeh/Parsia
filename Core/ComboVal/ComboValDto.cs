using DataLayer.Base;

namespace Parsia.Core.ComboVal
{
    public class ComboValDto : BaseDto
    {
        public long EntityId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public ComboValDto Parent { get; set; }

        public bool AdminOnly { get; set; }
    }
}