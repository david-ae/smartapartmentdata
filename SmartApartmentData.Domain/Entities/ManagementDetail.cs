using Nest;

namespace SmartApartmentData.Domain.Entities
{
    public class ManagementDetail
    {
        public long MgmtID { get; set; }
        [Text(Analyzer = "search_string", Name = nameof(Name))]
        public string Name { get; set; }
        public string Market { get; set; }
        public string State { get; set; }
    }
}
