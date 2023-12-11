// See https://aka.ms/new-console-template for more information

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rcetest;
using RestSharp;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        var options = new RestClientOptions("https://vyper2.remixproject.org/");
        services.AddScoped<RestClient>(s => new RestClient(options, useClientFactory: true));
    }).Build();

// var client = host.Services.GetRequiredService<HttpClient>();

var restClient = host.Services.GetRequiredService<RestClient>();

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
    Console.WriteLine("  --number\t\t or -n\t\tThe number of connections to open.");
};
Dictionary<string, string> valuesPassed = [];

if (args.Length > 0)
{
    RunThings.ProcessArgs(args, assembly!.ToString(), valuesPassed, returnHelpMessage, restClient);    
}