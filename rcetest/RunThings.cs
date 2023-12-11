using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp;

namespace rcetest;

internal static class RunThings
{
    [RequiresUnreferencedCode("Calls rcetest.RunThings.RunTest(Dictionary<String, String>, HttpClient)")]
    [RequiresDynamicCode("Calls rcetest.RunThings.RunTest(Dictionary<String, String>, HttpClient)")]
    public static void ProcessArgs(string[] args, string assembly, 
        Dictionary<string, string> valuesPassed, 
        Action returnHelpMessage,
        RestClient client)
    {
        if (args.Length == 1 && args[0].Contains("help"))
        {
            returnHelpMessage();
        }
        else if (args.Length == 1 && args[0].Contains("version"))
        {
            Console.WriteLine($"rcetest version {assembly}");
        }
        else if (args.Length > 0)
        {
            // capture all the values of each argument
            for(var idx = 0; idx < args.Length; idx++)
            {
                var keyidx = 0;
                var valueidx = 0;
                if (idx % 2 == 0 || idx == 0)
                {
                    keyidx = idx; 
                    valueidx = idx+1;
                }
                else
                {
                    valueidx = idx;
                }
                if (valueidx == idx)
                {
                    continue;
                }
                valuesPassed.Add(args[keyidx], args[valueidx]);
            }

            RunTest(valuesPassed, client);
        }
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
    private static void RunTest(Dictionary<string, string> valuesPassed, RestClient client)
    {
        var number = int.TryParse(valuesPassed["-n"], out var num) ? num : 1;
        var pload = new Payload();
        pload.Sources.ContractName.Content.Replace("\u0022", "");
        var payload = JsonSerializer.Serialize(pload, typeof(Payload), RCETestSerializerContext.Default);
        var tasks = new List<Task>();
        for (var i = 0; i < number; i++)
        {
            tasks.Add(SendCompileRequest(client, payload));
        }
        Task.WaitAll([..tasks]);
    }

    private static async Task<string> SendCompileRequest(RestClient client, string payload)
    {
        var request = new RestRequest("compile", Method.Post);
        request.AddJsonBody(new Payload(), ContentType.Json);
        try
        {
            var response = await client.ExecuteAsync(request).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var taskId = response.Content;
                await GetCompileResult(client, taskId).ConfigureAwait(false);
                return taskId;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                Console.WriteLine(response.Content);
                return string.Empty;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static async Task<string> GetCompileStatus(string taskId, RestClient client)
    {
        var cleanedTaskId = taskId.Replace("\"", "");
        var request = new RestRequest($"status/{cleanedTaskId}", Method.Get);
        var response = await client.GetAsync(request).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var result = response.Content;
            Console.WriteLine(result);
            return result.Replace("\"", "");
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            Console.WriteLine(response.Content);
            return string.Empty;
        }
    }
    
    
    private static async Task GetCompileResult(RestClient client, string taskId)
    {
        var cleanedTaskId = taskId.Replace("\"", "");
        var status = await GetCompileStatus(cleanedTaskId, client).ConfigureAwait(false);
        if(status != "SUCCESS") return;
        var request = new RestRequest($"artifacts/{cleanedTaskId}", Method.Get);
        var response = await client.GetAsync(request).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var result = response.Content;
            Console.WriteLine(result);
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            Console.WriteLine(response.Content);
        }
    }
}