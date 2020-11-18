using DataLayer.Base;
using Parsia.Core.ComboVal;

namespace Parsia.Core.Location
{
    public class LocationDto : BaseDto
    {
        public long EntityId { get; set; }
        public string Name { get; set; }
        public LocationDto Parent { get; set; }
        public ComboValDto Type { get; set; }
    }
}