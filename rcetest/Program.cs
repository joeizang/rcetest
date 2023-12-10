// See https://aka.ms/new-console-template for more information

using System.Reflection;

var assembly = Assembly.GetCallingAssembly().GetName().Version;


if (args.Length == 0)
{
    Console.WriteLine("No arguments provided. Use --help for more information.");
    return;
}
var returnHelpMessage = () =>
{
    Console.WriteLine("rcetest is a tool for testing the remote vyper compilation endpoint.");
    Console.WriteLine("Usage: rcetest [options] and options are separated by spaces.");
    Console.WriteLine("Options:");
    Console.WriteLine("  --help\t\tDisplay this help message.");
    Console.WriteLine("  --version\t\tDisplay the version of rcetest.");
    Console.WriteLine("  --endpoint\t\t or -e\t\tThe endpoint of the Hybrid Connection to test.");
    Console.WriteLine("  --number\t\t or -n\t\tThe number of connections to open.");
    Console.WriteLine("  --payload\t\t or -p\t\tThe size of the payload to send.");
};
Dictionary<string, string> valuesPassed = [];
Console.WriteLine($"{args.Length}");

var evaluateArgsPassed = () => {
    if (args.Length == 1 && args[0].Contains("help"))
    {
        returnHelpMessage();
    }
    else if (args.Length == 1 && args[0].Contains("version"))
    {
        Console.WriteLine($"rcetest version {assembly}");
    }
    else if (args.Length <= 6 && args.Length > 1)
    {
        foreach (var one in args)
        {
            // capture all the values of each argument
            var stripped = one.Split("-").Last().Split(" ");
            valuesPassed.Add(stripped[0], stripped[1]);
        }
        foreach(var v in valuesPassed)
        {
            Console.WriteLine($"{v.Key}: {v.Value}");
        }
    }
    else if (args.Length > 6)
    {
        Console.WriteLine("Too many arguments. Use --help for more information.");
    }
    else
    {
        Console.WriteLine("No arguments provided. Use --help for more information.");
    }
};

if (args.Length > 0)
{
    RunThings.ProcessArgs(args, assembly!.ToString(), valuesPassed, returnHelpMessage);
    // switch (args[0])
    // {
    //     case "--help":
    //         Console.WriteLine("rcetest is a tool for testing the remote vyper compilation endpoint.");
    //         Console.WriteLine("Usage: rcetest [options]");
    //         Console.WriteLine("Options:");
    //         Console.WriteLine("  --help\t\tDisplay this help message.");
    //         Console.WriteLine("  --version\t\tDisplay the version of rcetest.");
    //         Console.WriteLine("  --endpoint\t\t or -e\t\tThe endpoint of the Hybrid Connection to test.");
    //         Console.WriteLine("  --number\t\t or -n\t\tThe number of connections to open.");
    //         Console.WriteLine("  --payload\t\t or -p\t\tThe size of the payload to send.");
    //         return;
    //     case "--version":
    //         Console.WriteLine($"rcetest version {assembly}");
    //         return;
    //     case "--endpoint" or "-e":
    //         Console.WriteLine($"Endpoint: {args[1]}");
    //         return;
    //     case "--number" or "-n":
    //         Console.WriteLine($"Number: {args[1]}");
    //         return;
    //     case "--payload" or "-p":
    //         Console.WriteLine($"Payload: {args[1]}");
    //         return;
    //     default:
    //         Console.WriteLine("Invalid argument. Use --help for more information.");
    //         return;
    // }
    // evaluateArgsPassed();
    
}

public static class RunThings
{
    public static void ProcessArgs(string[] args, string assembly, Dictionary<string, string> valuesPassed, Action returnHelpMessage)
    {
        if (args.Length == 1 && args[0].Contains("help"))
        {
            returnHelpMessage();
        }
        else if (args.Length == 1 && args[0].Contains("version"))
        {
            Console.WriteLine($"rcetest version {assembly}");
        }
        else if (args.Length <= 6 && args.Length > 1)
        {
            // capture all the values of each argument
            for(var idx = 0; idx < args.Length; idx++)
            {
                var keyidx = 0;
                var valueidx = 0;
                if(idx % 2 == 0 || idx == 0) keyidx = idx;
                if(idx % 2 == 0 || idx == 0) valueidx = idx+1;
                else valueidx = idx;
                if (valueidx == idx)
                {
                    continue;
                }
                valuesPassed.Add(args[keyidx], args[valueidx]);
            }
            foreach(var v in valuesPassed)
            {
                Console.WriteLine($"{v.Key} {v.Value}");
            }
        }
        else if (args.Length > 6)
        {
            Console.WriteLine("Too many arguments. Use --help for more information.");
        }
        else
        {
            Console.WriteLine("No arguments provided. Use --help for more information.");
        }
    }
}
