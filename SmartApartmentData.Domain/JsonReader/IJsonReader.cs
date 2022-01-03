using SmartApartmentData.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartApartmentData.Domain.JsonReader
{
    public interface IJsonReader
    {
        IEnumerable<ManagementData> GetManagementData();
        IEnumerable<PropertyData> GetPropertyData();      
    }
}
