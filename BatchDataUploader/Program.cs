using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Common.Mapping;
using Common.Models;
using Nest;
using Newtonsoft.Json;

namespace BatchDataUploader
{
    public class Program
    {
        private static ElasticClient _client;

        public static async Task<int> Main(string[] args)
        {
            ConnectElasticDb();
            return await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async (CommandLineOptions opts) =>
                    {
                        try
                        {
                            await Upload(opts.Type, opts.Index, opts.Path);
                            return -1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            return -3; // Unhandled error
                        }
                    },
                    errs => Task.FromResult(-1));
        }

        private static void ConnectElasticDb()
        {
            var settings =
                new ConnectionSettings(new Uri(
                    "https://search-propertysearchtest-pvlw7vititpwbnkucgdygazmym.us-east-1.es.amazonaws.com"));
            settings.BasicAuthentication("admin", "MarinaGlupson2107@");
            settings.DisableDirectStreaming();
            settings.DefaultIndex("search");
            _client = new ElasticClient(settings);
        }

        private static async Task<int> Upload(string type, string index, string path)
        {
            if (type == "single")
                return await SingleUpload(index, path);

            return await BatchUpload(index, path);
        }

        private static async Task<int> BatchUpload(string index, string path)
        {
            Console.WriteLine("Started batch from " + path);
            var list = new List<PropertyMnmgt>();
            switch (index)
            {
                case "mgmt":
                    var mList = LoadManagementsJson(path);
                    Console.WriteLine("Count = " + mList.Count);
                    foreach (var mItem in mList)
                    {
                        var convertedMgmt = Mapper.MapFrom(mItem.Mgmt);
                        convertedMgmt.Type = "mgmt";
                        list.Add(convertedMgmt);
                    }
                    await BulkUpload(list);
                    break;
                case "property":
                    var pList = LoadPropertiesJson(path);
                    Console.WriteLine("Count = " + pList.Count);
                    foreach (var pItem in pList)
                    {
                        var convertedProperty = Mapper.MapFrom(pItem.Property);
                        convertedProperty.Type = "property";
                        list.Add(convertedProperty);
                        
                    }
                    await BulkUpload(list);
                    break;
            }
            return await Task.FromResult(-1);
        }

        private static Task<int> BulkUpload(IEnumerable<PropertyMnmgt> list)
        {
            var bulkAllObservable = _client.BulkAll(list, b => b
                    .Index("search")
                    .BackOffTime("30s")
                    .BackOffRetries(2)
                    .RefreshOnCompleted()
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                    .Size(1000)
                )
                .Wait(TimeSpan.FromMinutes(15), next => { Console.WriteLine("Finished?"); });

            return Task.FromResult<int>(-1);
        }

        private static async Task<int> SingleUpload(string index, string path)
        {
            Console.WriteLine("Started single.");
            try
            {
                PropertyMnmgt property;
                if (index == "property")
                {
                    var propertyInput = JsonConvert.DeserializeObject<PropertyInput>(path);
                    property = Mapper.MapFrom(propertyInput);
                    property.Type = index;
                    await Upload(property);
                }

                if (index == "mgmt")
                {
                    var managementInput = JsonConvert.DeserializeObject<ManagementInput>(path);
                    property = Mapper.MapFrom(managementInput);
                    property.Type = index;
                    await Upload(property);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return await Task.FromResult(-1);
        }

        private static async Task<IndexResponse> Upload(PropertyMnmgt data)
        {
            var response = await _client.IndexDocumentAsync(data);
            Console.WriteLine("Data written to " + response.Index);
            return response;
        }

        private static List<PropertyContainer> LoadPropertiesJson(string path)
        {
            var json = File.ReadAllText("../../../" + path);
            if (json == "")
                return null;

            var properties = JsonConvert.DeserializeObject<List<PropertyContainer>>(json);
            return properties;
        }

        private static List<ManagementContainer> LoadManagementsJson(string path)
        {
            var json = File.ReadAllText("../../../" + path);
            if (json == "")
                return null;

            var managements = JsonConvert.DeserializeObject<List<ManagementContainer>>(json);
            return managements;
        }
    }
}