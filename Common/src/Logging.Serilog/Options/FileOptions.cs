namespace Common.Logging.Serilog.Options
{
    internal class FileOptions
    {
        public bool Enabled { get; set; }
        public string Path { get; set; }
        public string Interval { get; set; }
    }
}