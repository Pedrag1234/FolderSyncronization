using CommandLine;
using FolderSyncronization;

class Program
{


    static void Main(string[] args)
    {
        Console.WriteLine(" _____     _     _           ____                             _              \r\n |  ___|__ | | __| | ___ _ __/ ___| _   _  ___ _ __ ___  _ __ (_)_______ _ __ \r\n | |_ / _ \\| |/ _` |/ _ \\ '__\\___ \\| | | |/ __| '__/ _ \\| '_ \\| |_  / _ \\ '__|\r\n |  _| (_) | | (_| |  __/ |   ___) | |_| | (__| | | (_) | | | | |/ /  __/ |   \r\n |_|  \\___/|_|\\__,_|\\___|_|  |____/ \\__, |\\___|_|  \\___/|_| |_|_/___\\___|_|   \r\n                                    |___/                                    ");
        Console.WriteLine("Press Ctrl-C to terminate Program");

        Options? parsedOptions = null;

        Parser.Default.ParseArguments<Options>(args)
              .WithParsed<Options>(opts => parsedOptions = opts)
              .WithNotParsed(errs => HandleParseErrors(errs));

        if (parsedOptions == null)
        {
            Console.WriteLine("[Error] - Failed to parse Command Line Arguments");
            return;
        }

        FolderSyncronizer sync = new FolderSyncronizer(parsedOptions);

         if (sync.Initialize() == -1)
         {
            Console.WriteLine("[Error] - Failed to Initialize Syncronizer");
            return;
         }

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("Terminating Program ...");
            sync.terminated = true;
            e.Cancel = true;
        };


        sync.Run();
        
    }


    static void HandleParseErrors(IEnumerable<Error> errs)
    {
        Console.WriteLine("errors {0}", errs.Count());
        if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
        {
            Console.WriteLine("Help or version requested.");
        }
        else
        {
            Console.WriteLine("Error parsing arguments.");
        }
    }
}