using DataLayer.Base;
using Parsia.Core.ComboVal;
using Parsia.Core.File;

namespace Parsia.Core.Person
{
    
    public class PersonDto : BaseDto
    {
        public long EntityId { get; set; }
        public string NationalCode { get; set; }
        public string PersianCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public double BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Description { get; set; }
        public string ExportationPlace { get; set; }
        public string CertificateSeries { get; set; }
        public bool LeftHanded { get; set; }
        public string Mobile { get; set; }
        public string EmergencyPhone { get; set; }
        public string Email { get; set; }
        public string IdentitySerialNumber { get; set; }
        public FileDto File { get; set; }
        public ComboValDto Sex { get; set; }
        public ComboValDto MilitaryServiceStatus { get; set; }
        public ComboValDto Education { get; set; }
        public ComboValDto BloodType { get; set; }
        public ComboValDto Nationality { get; set; }
        public ComboValDto LifeStatus { get; set; }
        public ComboValDto Citizenship { get; set; }
        public ComboValDto Religion { get; set; }
        public ComboValDto SubReligion { get; set; }
        public ComboValDto MaritalStatus { get; set; }
        public ComboValDto HousingSituation { get; set; }
        public ComboValDto HealthStatus { get; set; }
        public ComboValDto DisabilityType { get; set; }
    }

}