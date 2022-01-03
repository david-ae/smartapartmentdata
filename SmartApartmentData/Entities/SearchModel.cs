using System.Collections.Generic;

namespace SmartApartmentData.API.Entities
{
    public class SearchModel
    {
        public string SearchString { get; set; }
        public List<string> Market { get; set; }
    }
}
