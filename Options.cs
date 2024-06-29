using CommandLine;


namespace FolderSyncronization
{
    public class Options
    {
        [Option('s', "source", Required = true, HelpText = "Source path folder to be replicated")]
        public string? SourceFolder { get; set; }

        [Option('r', "replica", Required = true, HelpText = "Path to replicate source folder")]
        public string? ReplicaFolder { get; set; }

        [Option('i', "interval", Required = true, HelpText = "Interval to syncronize, interval in ms")]
        public int Interval { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file for logs")]
        public string? OutputFile { get; set; }
    }
}
