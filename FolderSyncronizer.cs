using NLog;

namespace FolderSyncronization
{
    public class FolderSyncronizer
    {
        private Options _options;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


        private FileAnalyzer _analyzer;
        public bool terminated {get;set;}

        public FolderSyncronizer(Options parsedOptions) { 
            _options = parsedOptions;
            terminated = false;
            _analyzer = new FileAnalyzer();
        }


        public void InitLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = _options.OutputFile };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;
        }

        public int Initialize()
        {
            InitLogger();
            _logger.Info("Initializing Folder Syncronizer");

            if (!Directory.Exists(_options.SourceFolder))
            {
                _logger.Error("Source Folder does not exist. Please ensure its the correct path");
                return -1;
            }

            if(!Directory.Exists(_options.ReplicaFolder))
            {
                _logger.Debug("Creating replica folder");
                Directory.CreateDirectory(_options.ReplicaFolder);
            }

            if(_options.Interval < 0 ||  _options.Interval >= int.MaxValue)
            {
                _logger.Error("Invalid Interval");
                return -1;
            }

            return 0;
        }

        public void Run()
        {
            while (!terminated)
            {
                this.SyncFolder(_options.SourceFolder,_options.ReplicaFolder);

                Thread.Sleep(_options.Interval);
            }
        }

        private void UpdateFiles(string sourceFolder, string replicaFolder, List<string> sourceFiles, List<string> replicaFiles)
        {
            // Copy files from source to replica
            foreach (var file in sourceFiles)
            {
                try
                {
                    string sourceFilePath = Path.Combine(sourceFolder, file);
                    string replicaFilePath = Path.Combine(replicaFolder, file);

                    // Copy the file only if it doesn't exist in the replica
                    if (!File.Exists(replicaFilePath))
                    {
                        _logger.Info($"Creating copy of file {file} to replica folder");
                        File.Copy(sourceFilePath, replicaFilePath, true);
                    }
                    else
                    {
                        // Copy the file if MD5 are not equal
                        if (!_analyzer.CompareFiles(sourceFilePath, replicaFilePath))
                        {
                            _logger.Info($"Copying modified file {file} to replica folder");
                            File.Copy(sourceFilePath, replicaFilePath, true);
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to copy/edit files: {file} - {ex}");
                    terminated = true;
                    return;
                }

            }
        }

        
        private void DeleteFiles(string replicaFolder, List<string> sourceFiles, List<string> replicaFiles)
        {
            //Delete Files that were remove in source
            foreach (var file in replicaFiles.Except(sourceFiles))
            {
                _logger.Info($"Deleting {file} from replica folder");
                try
                {
                    File.Delete(Path.Combine(replicaFolder, file));
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to remove files: {file} - {ex}");
                    terminated = true;
                    return;
                }

            }
        }

        private void DeleteFolders(string replicaFolder, List<string> sourceFolders, List<string> replicaFolders)
        {
            foreach (var folder in replicaFolders.Except(sourceFolders))
            {
                _logger.Info($"Deleting {folder} from replica folder");
                try
                {
                    Directory.Delete(Path.Combine(replicaFolder,folder),true);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to remove files: {folder} - {ex}");
                    terminated = true;
                    return;
                }

            }
        }

        private void SyncFolder(string sourceFolder, string replicaFolder)
        {
            //Get Files from current folder
            var sourceFiles = Directory.GetFiles(sourceFolder).Select(Path.GetFileName).ToList();
            var replicaFiles = Directory.GetFiles(replicaFolder).Select(Path.GetFileName).ToList();

            //Get SubFolders from current folder
            var sourceFolders = Directory.GetDirectories(sourceFolder).Select(Path.GetFileName).ToList();
            var replicaFolders = Directory.GetDirectories(replicaFolder).Select(Path.GetFileName).ToList();


            DeleteFiles(replicaFolder, sourceFiles, replicaFiles);
            UpdateFiles(sourceFolder, replicaFolder, sourceFiles, replicaFiles); 
            
            DeleteFolders(replicaFolder, sourceFolders, replicaFolders);

            foreach (var folder in sourceFolders)
            {
                string sourceSubFolderPath = Path.Combine(sourceFolder, folder);
                string replicaSubFolderPath = Path.Combine(replicaFolder, folder);


                if (!Directory.Exists(replicaSubFolderPath))
                {
                    try
                    {
                        _logger.Info($"Creating Sub Folder {folder}");
                        Directory.CreateDirectory(replicaSubFolderPath);
                    } catch (Exception ex) { _logger.Error(ex); }
                }

                SyncFolder(sourceSubFolderPath, replicaSubFolderPath);
            }

        }
    }
}
