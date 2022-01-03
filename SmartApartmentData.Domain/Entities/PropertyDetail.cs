using Nest;

namespace SmartApartmentData.Domain.Entities
{
    public class PropertyDetail
    {
        public long PropertyID { get; set; }
        [Text(Analyzer = "search_string", Name = nameof(Name))]
        public string Name { get; set; }
        [Text(Analyzer = "search_string", Name = nameof(FormerName))]
        public string StreetAddress { get; set; }
        public string FormerName { get; set; }
        public string City { get; set; }
        public string Market { get; set; }
        public string State { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
