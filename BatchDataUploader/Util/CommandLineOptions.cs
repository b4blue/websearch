using CommandLine;

namespace BatchDataUploader
{
    public class CommandLineOptions
    {
        [Option(shortName: 'i', longName: "index", Required = true, HelpText = "Index name: property or management", Default = "property")]
        public string Index { get; set; }

        [Option(shortName: 't', longName: "type", Required = true, HelpText = "Type of upload: Single or Batch.", Default = "batch")]
        public string Type { get; set; }
        
        [Option(shortName: 'p', longName: "path", Required = true, HelpText = "If type is batch this is file reference. If single, this is json.", Default = "")]
        public string Path { get; set; }
    }
}