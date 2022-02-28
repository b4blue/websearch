using System;
using System.Collections.Generic;
using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.Configuration;
using Nest;

namespace ElasticSearch
{
    public class ElasticSearchService : ISearchService
    {
        private readonly ElasticClient _client;

        public ElasticSearchService(IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration.GetSection("AWS:Endpoint").Value));
            settings.BasicAuthentication(configuration.GetSection("AWS:Username").Value, configuration.GetSection("AWS:Password").Value);
            settings.DisableDirectStreaming();
            _client = new ElasticClient(settings);
        }

        public IEnumerable<PropertyMnmgt> GetData(string searchTerm, string market, int size)
        {
            var response = _client.Search<PropertyMnmgt>(s => s
                .From(0)
                .Size(size)
                .Index("search")
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            bs => bs
                                .QueryString(st => st
                                    .Fields(new[] { "name", "streetAddress", "formerName" })
                                    .Analyzer("whitespace")
                                    .Analyzer("stop")
                                    .Query(searchTerm))
                        )
                        .Filter(fi => fi
                            .Match(r => r
                                .Field(f => f.Market)
                                .Query(market)
                            )
                        )
                    )
                )
            );
            return response.Documents;
        }
    }
}