using System.Collections.Generic;
using OpenTelemetry.Exporter;

namespace Common.Diagnostics
{
    public class OTelOptions
    {
        public IEnumerable<string> Services { get; set; }
        public ZipkinExporterOptions ZipkinExporterOptions { get; set; }
        public JaegerExporterOptions JaegerExporterOptions { get; set; }
    }
}