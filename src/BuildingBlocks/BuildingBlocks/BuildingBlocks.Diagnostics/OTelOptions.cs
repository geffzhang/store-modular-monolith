using System.Collections.Generic;
using OpenTelemetry.Exporter;

namespace BuildingBlocks.Diagnostics
{
    public class OTelOptions
    {
        public IEnumerable<string> Services { get; set; }
        public ZipkinExporterOptions ZipkinExporterOptions { get; set; }
        public JaegerExporterOptions JaegerExporterOptions { get; set; }
    }
}