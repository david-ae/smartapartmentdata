using Elasticsearch.Net;
using Nest;
using SmartApartmentData.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartApartmentData.Infrastructure.Services
{
    public interface IInfrastructureService
    {
        PutIndexTemplateResponse createPropertiesAnalyzers();
        PutIndexTemplateResponse createManagementsAnalyzers();
        Task<ISearchResponse<ManagementData>> SearchManagement(string searchString, List<string> market);
        Task<ISearchResponse<PropertyData>> SearchProperty(string searchString, List<string> market);
        bool ImportPropertyData(IEnumerable<PropertyData> propertyData);
        bool ImportManagementData(IEnumerable<ManagementData> managementData);
    }
}
