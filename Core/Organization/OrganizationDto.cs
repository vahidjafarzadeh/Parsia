using DataLayer.Base;
using Parsia.Core.ComboVal;
using Parsia.Core.File;
using Parsia.Core.Location;

namespace Parsia.Core.Organization
{
    public class OrganizationDto : BaseDto
    {
        public long EntityId { get; set; }
        public long? ParentId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Name { get; set; }
        public string Order { get; set; }
        public string Area { get; set; }
        public string NumberOfFloors { get; set; }
        public string NumberOfRooms { get; set; }
        public string TelPhone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string SmsNumber { get; set; }
        public string Description { get; set; }
        public double? YearOfConstruction { get; set; }
        public double? EstablishingYear { get; set; }
        public string AboutUs { get; set; }
        public LocationDto Province { get; set; }
        public LocationDto City { get; set; }
        public ComboValDto OrganizationStatus { get; set; }
        public ComboValDto OrganizationType { get; set; }
        public ComboValDto OrganizationGrade { get; set; }
        public ComboValDto OrganizationOwnershipType { get; set; }
        public ComboValDto OrganizationRoadType { get; set; }
        public FileDto Logo { get; set; }
        public OrganizationDto Parent { get; set; }
    }
}