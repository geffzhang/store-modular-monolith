using System.Collections.Generic;

namespace Common.Logging.Serilog
{
    internal class LoggerOptions
    {
        public string Level { get; set; }
        public IDictionary<string, string> MinimumLevelOverrides { get; set; }
        public IEnumerable<string> ExcludePaths { get; set; }
        public IEnumerable<string> ExcludeProperties { get; set; }
        public IDictionary<string, object> Tags { get; set; }
        public bool UseSeq { get; set; }
        public bool UseStackify { get; set; }
        public bool UseElasticSearch { get; set; }
        public ElasticSearchLoggingOptions ElasticSearchLoggingOptions { get; set; }
        public SeqLoggingOptions SeqOptions { get; set; }
    }

    internal class ElasticSearchLoggingOptions
    {
        public string Url { get; set; }
    }

    internal class SeqLoggingOptions
    {
        public string Url { get; set; }
    }
}