using Nest;
using SmartApartmentData.Domain.Entities;
using SmartApartmentData.Infrastructure.Settings;
using SmartApartmentData.Infrastructure.Settings.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartApartmentData.Infrastructure.Services
{
    public class InfrastructureService : IInfrastructureService
    {
        private readonly AWSSearchContext _context;
        private bool _managementIndexCreated = false;
        private bool _propertyIndexCreated = false;

        public InfrastructureService(AWSSettings settings)
        {
            _context = new AWSSearchContext(settings);
        }

        public PutIndexTemplateResponse createManagementsAnalyzers()
        {
            var managementIndexResponse = _context.Client.Indices.PutTemplate("managements-03", i => i
                .IndexPatterns("managements-*")
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(a => a
                         .Custom("search_string", ca => ca
                                 .Filters("lowercase", "stop")
                                 .Tokenizer("search_string")))
                        .Tokenizers(tk => tk
                                .EdgeNGram("search_string", eg => eg
                                            .MinGram(2)
                                            .MaxGram(20)
                                            .TokenChars(TokenChar.Letter)))
                        .TokenFilters(tf => tf
                                    .Stop("stop", l => l
                                        .StopWords("_english_")))))
                .Map<ManagementData>(m => m.AutoMap().Properties(p => p.Text(t => t.Name(n => n.Mgmt.Name).Analyzer("search_string")))));

            return managementIndexResponse;
        }

        public PutIndexTemplateResponse createPropertiesAnalyzers()
        {
            var propertyIndexResponse = _context.Client.Indices.PutTemplate("properties-03", i => i
                .IndexPatterns("properties-*")
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(a => a
                         .Custom("search_string", ca => ca
                                 .Filters("lowercase", "stop")
                                 .Tokenizer("search_string")))
                        .Tokenizers(tk => tk
                                .EdgeNGram("search_string", eg => eg
                                            .MinGram(3)
                                            .MaxGram(20)
                                            .TokenChars(TokenChar.Letter)))
                        .TokenFilters(tf => tf
                                    .Stop("stop", l => l
                                        .StopWords("_english_")))))                
                .Map<PropertyData>(m => m.AutoMap().Properties(p => p.Text(t => t.Name(n => n.Property.FormerName).Analyzer("search_string"))))
                .Map<PropertyData>(m => m.AutoMap().Properties(p => p.Text(t => t.Name(n => n.Property.Name).Analyzer("search_string")))));

            return propertyIndexResponse;
        }
        public async Task<ISearchResponse<ManagementData>> SearchManagement(string searchString, List<string> market)
        {
            var managements = await _context.Client.SearchAsync<ManagementData>(s => s.
                                        From(0).
                                        Size(25).                                        
                                        Index("managements-03")
                                            .Query(q => q
                                                .Bool(b => b
                                                    .Should(
                                                        mu => mu.MatchPhrase(m => m.Field(f => f.Mgmt.Name)
                                                        .Query(searchString))
                                                        )
                                                    .Must(
                                                        fl => fl.Terms(t => t.Field(tf => tf.Mgmt.Market.Suffix("keyword"))
                                                        .Terms(market))
                                                        )
                                                    )
                                                ).Sort(s => s.Descending(SortSpecialField.Score))
                                            );

            return managements;
        }

        public async Task<ISearchResponse<PropertyData>> SearchProperty(string searchString, List<string> market)
        {

            var properties = await _context.Client.SearchAsync<PropertyData>(s => s.
                                    From(0).
                                    Size(25).
                                    Index("properties-03")
                                        .Query(q => q
                                            .Bool(b => b
                                                .Should(
                                                    mu => mu.Match(m => m.Field(f => f.Property.Name)
                                                    .Query(searchString)),
                                                    mu => mu.MatchPhrase(m => m.Field(f => f.Property.StreetAddress)
                                                    .Query(searchString))
                                                    )
                                                .Must(
                                                    fl => fl.Terms(t => t.Field(tf => tf.Property.Market.Suffix("keyword"))
                                                    .Terms(market))
                                                    )
                                                )
                                            ).Sort(s => s.Descending(SortSpecialField.Score))
                                        );

            return properties;
        }

        public bool ImportManagementData(IEnumerable<ManagementData> managementData)
        {  
            var bulkAllManagmentsObservable = _context.Client.BulkAll(managementData, b => b
                .Index("managements-03")
                .BackOffTime("60s")
                .BackOffRetries(2)
                .RefreshOnCompleted()
                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(1000)
            )
            .Wait(TimeSpan.FromMinutes(15), next => {});

            _managementIndexCreated = bulkAllManagmentsObservable.TotalNumberOfFailedBuffers > 0 ? false : true;

            return _managementIndexCreated;
        }

        public bool ImportPropertyData(IEnumerable<PropertyData> propertyData)
        {
            var bulkAllPropertiesObservable = _context.Client.BulkAll(propertyData, b => b
                .Index("properties-03")
                .BackOffTime("60s")
                .BackOffRetries(2)
                .RefreshOnCompleted()
                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(1000)
            )
            .Wait(TimeSpan.FromMinutes(10), next => {});

            _propertyIndexCreated = bulkAllPropertiesObservable.TotalNumberOfFailedBuffers > 0 ? false : true;

            return _propertyIndexCreated;
        }
    }
}
