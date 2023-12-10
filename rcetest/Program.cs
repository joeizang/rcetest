// See https://aka.ms/new-console-template for more information

using System.Reflection;
using rcetest;

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

if (args.Length > 0)
{
    RunThings.ProcessArgs(args, assembly!.ToString(), valuesPassed, returnHelpMessage);    
}