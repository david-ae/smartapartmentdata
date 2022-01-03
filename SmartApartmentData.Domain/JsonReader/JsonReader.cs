using Newtonsoft.Json;
using SmartApartmentData.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace SmartApartmentData.Domain.JsonReader
{
    public class JsonReader : IJsonReader
    {
        public IEnumerable<ManagementData> GetManagementData()
        {
            var managementList = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Data\\{"mgmt.json"}");
            var managementListString = File.ReadAllText(managementList);
            var managements = JsonConvert.DeserializeObject<IEnumerable<ManagementData>>(managementListString);

            return managements;
        }

        public IEnumerable<PropertyData> GetPropertyData()
        {
            var propertyList = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Data\\{"properties.json"}");
            var propertyListString = File.ReadAllText(propertyList);
            var properties = JsonConvert.DeserializeObject<IEnumerable<PropertyData>>(propertyListString);

            return properties;
        }
    }
}
